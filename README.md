# Mummy's Curse

## Introduction

**Mummys' Curse** is a mixed reality multiplay escape room experience. Players need to observe carefully, look for clues, and work together to solve a series of puzzles and riddle in order to break the mummy's curse and escape from the tomb.

The game uses passthrough and occlusion to blend the virtual and real worlds, and utilizes a variety of physical props and sensors to enhance immersion.

![poster](https://drive.google.com/drive/folders/19A3F6wCXQ9Vh_v1lKL9-732FYGK7e6Bt)

## Design process (TODO)

[**Design Folder**](https://drive.google.com/drive/folders/1O-wiWoeiYen7kSCbjZMcCvr8gcNXPC_h?usp=sharing)

[**Design Document**](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?usp=sharing)

### Phase 1: Mafia

### Phase 2: Mummy's Curse 1.0

### Phase 3: Mummy's Curse 2.0

## Features (TODO)

1. Overlay
2. MQTT Communication
3. Hall-Effect Sensor Based Props
4. Sensor Calibration
5. QR Code Redirect

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

## Usage (TODO)

Some clips of game.

## References

[Assets List](https://docs.google.com/document/d/1rfbI2MNi-GRsnRO2vtrxWCsdphHY01Io0in4PouiIhg/edit?tab=t.7qvqmomuc5aj)

[MQTT Tools Code](https://gitea.dsv.su.se/ExtralityLab/se.su.dsv.extralitylab.unity)

## Contributors

[Fernando Valcazara](feda6263@student.su.se)

[Johanna Källström](joka3246@student.su.se)

[Li Zijie](zili2900@student.su.se)

[Jasmine Shahnavazi](jash2879@student.su.se)