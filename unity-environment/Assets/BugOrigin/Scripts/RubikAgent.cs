using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RubikAgent : Agent
{
    [SerializeField]
    GameObject cubePrefab;

    [SerializeField]
    Transform[] nodeArr;

    Transform[,,] cubeTensor = new Transform[3, 3, 3];
    GameObject dynamicCube;

    Transform[,,] tensorTrans;

    [SerializeField]
    private Text text;

    int[,,] goalState = new int[3, 3, 3];
    int[,,] currentState = new int[3, 3, 3];
    int[,,] stateTrans = new int[3, 3, 3];

    int evolved;
    int solved;

    List<int> upList;
    List<int> downList;
    List<int> leftList;
    List<int> rightList;
    List<int> forwardList;
    List<int> backList;
    void Start()
    {
        upList = new List<int>();
        downList = new List<int>();
        leftList = new List<int>();
        rightList = new List<int>();
        forwardList = new List<int>();
        backList = new List<int>();

        tensorTrans = new Transform[3, 3, 3];
        int i = 0;
        //创建魔方
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    i++;
                    dynamicCube = Instantiate(cubePrefab, new Vector3(x - 1f, y - 1f, z - 1f), Quaternion.identity, transform);
                    dynamicCube.name = i.ToString();
                    cubeTensor[x, y, z] = dynamicCube.transform;
                    goalState[x, y, z] = i;
                    currentState[x, y, z] = i;
                }
            }
        }

        //加进6个方位的初始值
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    upList.Add(goalState[x, 2, z]);
                    downList.Add(goalState[x, 0, z]);
                    leftList.Add(goalState[0, y, z]);
                    rightList.Add(goalState[2, y, z]);
                    forwardList.Add(goalState[x, y, 2]);
                    backList.Add(goalState[x, y, 0]);
                }
            }
        }
    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    state.Add(currentState[x, y, z]);
                }
            }
        }
        return state;
    }
    public override void AgentStep(float[] act)
    {
        if (text != null)
            text.text = string.Format("Solved:{0}", solved);
        reward = -0.01f;
        int action = Mathf.FloorToInt(act[0]);
        RubikBehavior(action);
        if (JudgeUp())//|| JudgeDown() || JudgeLeft() || JudgeRight() || JudgeForward() || JudgeBack())
        {
            //evolved++;
            reward = 1;
            solved++;
            //Debug.Break();
            done = true;
        }
        if (Judge())
        {
            solved++;
            reward = 10;
            done = true;
        }
    }


    public override void AgentReset()

    {
        Restart();
        //RubikBehavior(5);
    }

    void Restart()
    {
        for (int i = 0; i < 100; i++)
        {
            RubikBehavior(Random.Range(0, 6));
        }
    }

    bool JudgeUp()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (!upList.Contains(currentState[x, 2, z]))
                    return false;
            }
        }
        return true;
    }
    bool JudgeDown()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (!downList.Contains(currentState[x, 0, z]))
                    return false;
            }
        }
        return true;
    }
    bool JudgeLeft()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (!leftList.Contains(currentState[0, y, z]))
                    return false;
            }
        }
        return true;
    }
    bool JudgeRight()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (!rightList.Contains(currentState[2, y, z]))
                    return false;
            }
        }
        return true;
    }
    bool JudgeForward()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (!forwardList.Contains(currentState[x, y, 2]))
                    return false;
            }
        }
        return true;
    }
    bool JudgeBack()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (!backList.Contains(currentState[x, y, 0]))
                    return false;
            }
        }
        return true;
    }

    bool Judge()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (currentState[x, y, z] != goalState[x, y, z])
                    {
                        return false;
                    }

                }
            }
        }
        return true;
    }
    //魔方行为
    void RubikBehavior(int n)
    {
        switch (n)
        {
            case 0:
                nodeArr[0].rotation = Quaternion.identity;
                //up层矩阵转置
                for (int x = 0; x < 3; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[x, 2, z].parent = nodeArr[0];
                        tensorTrans[x, 2, z] = cubeTensor[z, 2, x];
                        stateTrans[x, 2, z] = currentState[z, 2, x];
                    }
                }
                //魔方具象旋转
                nodeArr[0].eulerAngles = new Vector3(0, 90, 0);
                //对调首尾列 完成up层矩阵抽象旋转
                for (int x = 0; x < 3; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[x, 2, z] = tensorTrans[x, 2, 2 - z];
                        currentState[x, 2, z] = stateTrans[x, 2, 2 - z];
                        cubeTensor[x, 2, z].parent = transform;
                    }
                }
                nodeArr[0].rotation = Quaternion.identity;
                break;
            case 1:
                nodeArr[1].rotation = Quaternion.identity;
                //Down层矩阵转置
                for (int x = 0; x < 3; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[x, 0, z].parent = nodeArr[1];
                        tensorTrans[x, 0, z] = cubeTensor[z, 0, x];
                        stateTrans[x, 0, z] = currentState[z, 0, x];
                    }
                }
                nodeArr[1].eulerAngles = new Vector3(0, 90, 0);
                for (int x = 0; x < 3; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[x, 0, z] = tensorTrans[x, 0, 2 - z];
                        currentState[x, 0, z] = stateTrans[x, 0, 2 - z];
                        cubeTensor[x, 0, z].parent = transform;
                    }
                }
                nodeArr[1].rotation = Quaternion.identity;
                break;
            case 2:
                //Left层矩阵转置
                nodeArr[2].rotation = Quaternion.identity;
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[0, y, z].parent = nodeArr[2];
                        tensorTrans[0, y, z] = cubeTensor[0, z, y];
                        stateTrans[0, y, z] = currentState[0, z, y];
                    }
                }
                nodeArr[2].eulerAngles = new Vector3(-90, 0, 0);
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[0, y, z] = tensorTrans[0, y, 2 - z];
                        currentState[0, y, z] = stateTrans[0, y, 2 - z];
                        cubeTensor[0, y, z].parent = transform;
                    }
                }
                nodeArr[2].rotation = Quaternion.identity;
                break;
            case 3:
                //Right层矩阵转置
                nodeArr[3].rotation = Quaternion.identity;
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[2, y, z].parent = nodeArr[3];
                        tensorTrans[2, y, z] = cubeTensor[2, z, y];
                        stateTrans[2, y, z] = currentState[2, z, y];
                    }
                }
                nodeArr[3].eulerAngles = new Vector3(-90, 0, 0);
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        cubeTensor[2, y, z] = tensorTrans[2, y, 2 - z];
                        currentState[2, y, z] = stateTrans[2, y, 2 - z];
                        cubeTensor[2, y, z].parent = transform;
                    }
                }
                nodeArr[3].rotation = Quaternion.identity;
                break;
            case 4:
                //Forward层矩阵转置
                nodeArr[4].rotation = Quaternion.identity;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cubeTensor[x, y, 2].parent = nodeArr[4];
                        tensorTrans[x, y, 2] = cubeTensor[y, x, 2];
                        stateTrans[x, y, 2] = currentState[y, x, 2];
                    }
                }
                nodeArr[4].eulerAngles = new Vector3(0, 0, -90);
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cubeTensor[x, y, 2] = tensorTrans[x, 2 - y, 2];
                        currentState[x, y, 2] = stateTrans[x, 2 - y, 2];
                        cubeTensor[x, y, 2].parent = transform;
                    }

                }
                nodeArr[4].rotation = Quaternion.identity;
                break;
            case 5:
                //Back层矩阵转置
                nodeArr[5].rotation = Quaternion.identity;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cubeTensor[x, y, 0].parent = nodeArr[5];
                        tensorTrans[x, y, 0] = cubeTensor[y, x, 0];
                        stateTrans[x, y, 0] = currentState[y, x, 0];
                    }
                }
                nodeArr[5].eulerAngles = new Vector3(0, 0, -90);
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        cubeTensor[x, y, 0] = tensorTrans[x, 2 - y, 0];
                        currentState[x, y, 0] = stateTrans[x, 2 - y, 0];
                        cubeTensor[x, y, 0].parent = transform;
                    }
                }
                nodeArr[5].rotation = Quaternion.identity;
                break;
            default:
                return;
        }
    }

}
