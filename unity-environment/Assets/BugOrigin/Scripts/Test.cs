using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject cubePrefab;

    [SerializeField]
    Transform[] nodeArr;

    Transform[,,] cubeTensor = new Transform[3, 3, 3];
    GameObject dynamicCube;

    Transform[,,] tensorTrans;


    int[,,] goalState = new int[3, 3, 3];
    int[,,] currentState = new int[3, 3, 3];
    int[,,] stateTrans = new int[3, 3, 3];

    int left = 0;
    int right = 0;
    int forward = 0;
    int back = 0;
    // Use this for initialization
    void Start()
    {
        tensorTrans = new Transform[3, 3, 3];
        int i = 0;
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
    }
    // Update is called once per frame
    void Update()
    {
        Action();
        Debug.Log(Judge());
    }
    void Action()
    {
        //旋转up层
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
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
            nodeArr[0].rotation = Quaternion.Euler(0, 90, 0);
            //对调首尾列 完成up层矩阵抽象旋转
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[x, 2, z] = tensorTrans[x, 2, 2 - z];
                    currentState[x, 2, z] = stateTrans[x, 2, 2 - z];
                    cubeTensor[x, 2, z].parent = transform;
                    //Debug.Log(currentState[x, 2, z]);
                }
            }
        }

        //旋转Down层
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[x, 0, z].parent = nodeArr[1];
                    tensorTrans[x, 0, z] = cubeTensor[z, 0, x];
                    stateTrans[x, 0, z] = currentState[z, 0, x];
                }
            }
            nodeArr[1].rotation = Quaternion.AngleAxis(nodeArr[1].localEulerAngles.y + 90, Vector3.up);
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[x, 0, z] = tensorTrans[x, 0, 2 - z];
                    currentState[x, 0, z] = stateTrans[x, 0, 2 - z];
                    cubeTensor[x, 0, z].parent = transform;
                }
            }
        }


        //旋转Left层
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            nodeArr[2].rotation = Quaternion.identity;
            Debug.Log(nodeArr[0].rotation.eulerAngles.x);
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[0, y, z].parent = nodeArr[2];
                    tensorTrans[0, y, z] = cubeTensor[0, z, y];
                    stateTrans[0, y, z] = currentState[0, z, y];
                }
            }
            nodeArr[2].rotation = Quaternion.Euler(-90, 0, 0);
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[0, y, z] = tensorTrans[0, y, 2 - z];
                    currentState[0, y, z] = stateTrans[0, y, 2 - z];
                    cubeTensor[0, y, z].parent = transform;
                }
            }

        }

        //旋转Right层
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            right++;
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[2, y, z].parent = nodeArr[3];
                    tensorTrans[2, y, z] = cubeTensor[2, z, y];
                    stateTrans[2, y, z] = currentState[2, z, y];
                }
            }
            if (right > 3)
            {
                right = 0;
            }
            nodeArr[3].rotation = Quaternion.AngleAxis(right * 90, Vector3.left);
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeTensor[2, y, z] = tensorTrans[2, y, 2 - z];
                    currentState[2, y, z] = stateTrans[2, y, 2 - z];
                    cubeTensor[2, y, z].parent = transform;
                }
            }

        }

        //旋转Forward层
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
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
            nodeArr[4].rotation = Quaternion.Euler(0, 0, -90);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    cubeTensor[x, y, 2] = tensorTrans[x, 2 - y, 2];
                    currentState[x, y, 2] = stateTrans[x, 2 - y, 2];
                    cubeTensor[x, y, 2].parent = transform;
                }
            }
        }

        //旋转Back层
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            back++;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    cubeTensor[x, y, 0].parent = nodeArr[5];
                    tensorTrans[x, y, 0] = cubeTensor[y, x, 0];
                    stateTrans[x, y, 0] = currentState[y, x, 0];
                }
            }
            if (back > 3)
            {
                back = 0;
            }
            nodeArr[5].rotation = Quaternion.AngleAxis(back * 90, Vector3.back);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    cubeTensor[x, y, 0] = tensorTrans[x, 2 - y, 0];
                    currentState[x, y, 0] = stateTrans[x, 2 - y, 0];
                    cubeTensor[x, y, 0].parent = transform;
                }
            }
        }
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

}
