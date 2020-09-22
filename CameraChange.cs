using System.Collections;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public GameObject firstPersonMainCam;
    public GameObject thirdPersonMainCam;

    private int camMode;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            //Set camMode
            if(camMode == 1)
            {
                camMode = 0;
            }
            else
            {
                camMode += 1;
            }

            //Switch camera based on camMode
            StartCoroutine(CamChange());
        }
    }

    //Coroutine used to avoid glitches as a result of camera switching
    IEnumerator CamChange()
    {
        //Ensure that only one set of cams is on at a time
        yield return new WaitForSeconds(0.01f);

        //Default cam, first person mode
        if(camMode == 0)
        {
            firstPersonMainCam.SetActive(true);
            thirdPersonMainCam.SetActive(false);
        }

        //Alternate cam, third person mode
        if(camMode == 1)
        {
            firstPersonMainCam.SetActive(false);
            thirdPersonMainCam.SetActive(true);
        }
    }
}
