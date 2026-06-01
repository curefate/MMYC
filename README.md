# Mummy's Curse

## Introduction

**Mummys' Curse** is a mixed reality multiplay escape room experience. Players need to observe carefully, look for clues, and work together to solve a series of puzzles and riddle in order to break the mummy's curse and escape from the tomb.

The game uses passthrough and occlusion to blend the virtual and real worlds, and utilizes a variety of physical props and sensors to enhance immersion.

<img width="1370" height="1943" alt="image" src="https://github.com/user-attachments/assets/ef4dfa7e-d328-4b71-9668-dd27558c30a6" />

![poster](https://drive.google.com/drive/folders/19A3F6wCXQ9Vh_v1lKL9-732FYGK7e6Bt)

## Design process

[**Design Folder**](https://drive.google.com/drive/folders/1O-wiWoeiYen7kSCbjZMcCvr8gcNXPC_h?usp=sharing)

### Phase 1: Mafia

[**Design Document**](https://docs.google.com/document/d/1Ehma-Y1a294HS1l4nNsgTa0KNRkZo7E6IM7gJ1hSYZs/edit?usp=sharing)

The very beginning idea of us. The idea is about assigning different roles to players while encouraging competition among them to partially replace cooperation, find the mafia hidden in the crowd or complete the conspiracy to win the game.

But we eventually abandoned the idea. The main reason was traditional mafia game require **at least 7 players** for a good gaming experience. In our case, the amount of HMDs and management of players can be a unignoreable problem. We tried designed a new rule based on traditional game specific for less players, but it is too complex for a 10-15 mins demonstration.

### Phase 2: Mummy's Curse 1.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The first version of our Mummy's Curse idea. The main feature is the whole game is divided into 9 different rooms, with different puzzles and riddles. We disigned 3 color-based puzzles and 3 riddles, if player give the wrong answer of the riddle, one of them will lose color, so they need to collabrate more with each other. Players can switch into different rooms through the minimap in the middle of room, specifically, when they press the button, the overlay wall and digital objects will be replaced, we want players can have the feeling of advanture like **Tomb Riders** through this.

We designed weight scale puzzle, mirror reflection beam puzzle, flame hand puzzle, and three egyption style riddle in this version, which you can find in document.

While after supervision, we decided to redesign this version, because for a mixed-reality experience, there are too less tangible interaction, basically we can did the same thing in only VR. (Although we still think it is good!)

### Phase 3: Mummy's Curse 2.0

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

The current version of the game. We removed the minimap and room switch mechanism because it actually limit us to have tangible, physicial object in the game space. The weight scale and flame hand puzzles are inherited into this version, and be modified to be more tangible, for example, the weight of scale become physical props. The whole experience right now is more like an escape room experience, which players need to observe the whole room carefully, solve sequence of puzzles, and escape from the chamber of mummy.

## Features (TODO fill images)

1. Overlay

    Occlusion and Passthrough are used in project to blend the virtual and physical space. Player can see virtual object and real physical object at same time in correct geometry relationship.

    ![screenshot]()

2. MQTT Communication

    MQTT to facilitate communication between the sensors, actuators and the game session (hanlded by esp32). Specifically, this includes the values of hall effect sensors, signals to leds, language options and console monitoring.

    ![screenshot]()

3. Hall-Effect Sensor Based Props

    We designed a prop structure based on a hall effect sensor. Specifically, by controlling the distance between the magnet inside the prop and its bottom, the difference between reading value and baseline value can be used to define different prop types.

    ![screenshot]()

4. Sensor Calibration

    Because the magnetic field can vary depending on the environment and time, the baseline value of the hall effect sensor may differ. Therefore, the type of prop cannot be determined solely by a fixed interval; instead, the difference from the baseline should be used (as mentioned earlier). Furthermore, the baseline value needs to be recalibrated each time the game starts.

    ```csharp
    private IEnumerator Routine_Calibration()
    {
        yield return new WaitForSeconds(3f);
        var baseLine = (MQTTProcessor.Instance.Hall_0 + MQTTProcessor.Instance.Hall_1 + MQTTProcessor.Instance.Hall_2 + MQTTProcessor.Instance.Hall_3 + hall4) / 5f;
        MQTTProcessor.Instance.PublishMessage("MMYC/hall_base", Mathf.RoundToInt(baseLine).ToString());
        yield return null;
    }
    ```

5. QR Code Redirect

    We use the [trackable QR code](https://developers.meta.com/horizon/documentation/unity/unity-mr-utility-kit-qrcode-detection) feature of the Meta SDK to redirect the entire scene. When any player scans the corresponding QR code, the scene for all players will be redirected to correct position and direction, to make sure all players stay in same virtual space.

    [Source Code](Assets\Workspace\Scripts\QRRelocation.cs).

6. Multilingual

    We support both english and swedish version of text and voices. It controls by the MQTT signal: `MQTTProcessor.Instance.Language`

    ![screenshot]()

    ![screenshot]()
.
## Installation

Support devices: **Meta Quest 3/3s**

Unity Version: **6000.3.10.f1**

**Step 1:** There are two ways to get the installation file.

- Download from [release](https://github.com/curefate/MMYC/releases).
- `git clone https://github.com/curefate/MMYC.git`, clone this repository, open it in Unity using 6000.3.10.f1 or above, switch platform to Android, then compile.

**Step 2:** Install the .apk through Meta Quest Developer Hub, you will need a developer account to install unknown source application, see [details](https://developers.meta.com/horizon/documentation/native/android/mobile-device-setup/).

**Additioanl Steps:**

1. 3D print all props using models in design folder.
2. Upload arduino codes to your esp32, source code files are in /Assets/Workplace/Arduino.
3. Connect hall effect sensors to A0-A4. Leds are optional.

## Usage (TODO fill images)

Some clips of game.

### Puzzle 1

### Puzzle 2

### Puzzle 3

### Puzzle 4 (Riddle)

## References

[Assets List](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?tab=t.7qvqmomuc5aj)

[MQTT Tools Code](https://gitea.dsv.su.se/ExtralityLab/se.su.dsv.extralitylab.unity)

[Meta SDK](https://developers.meta.com/horizon/develop/unity/)

## Contributors

[Fernando Valcazara](fernandovalcazara@gmail.com)

[Johanna Källström](johannackallstrom@gmail.com)

[Li Zijie](curefate@outlook.com)

[Jasmine Shahnavazi](jasminesh31@gmail.com)
