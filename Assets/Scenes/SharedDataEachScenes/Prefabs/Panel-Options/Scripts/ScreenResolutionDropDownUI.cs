using System.Linq;
using UnityEngine;

namespace Scenes.SharedDataEachScenes.Prefabs.Scripts
{
    public class ScreenResolutionDropDownUI : MonoBehaviour
    {
        public void SelectedResolution(int index)
        {
            // index 0: 800x600px
            // index 1: 1280x720px
            // index 2: 1920x1080px
            // index 3: FullScreen

            // 게임 실행 중 화면이 꺼지지 않도록 설정
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            switch (index)
            {
                case 0: Screen.SetResolution(800, 600, false); break;
                case 1: Screen.SetResolution(1280, 720, false); break;
                case 2: Screen.SetResolution(1920, 1080, false); break;
                case 3:
                {
                    var supportedResolutions = Screen.resolutions;
                    var maxWidth = supportedResolutions.Max(resolution => resolution.width);
                    var maxResolution = supportedResolutions.FirstOrDefault(resolution => resolution.width == maxWidth);
                    Screen.SetResolution(maxResolution.width, maxResolution.height, true);
                    break;
                }
            }
        }
    }
}
