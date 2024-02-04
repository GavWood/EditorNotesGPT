[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

# EditorNotesGPT

Creates a notes tab for use inside the Editor which is powered by Chat-GPT

- [How to use](#how-to-use)
- [Install](#install)
  - [via Git URL](#via-git-url)
  - Copy Editor scripts to your Unity project. 
- [Configuration](#configuration)

## How to use

Using the BaaWolf menu at the top of your Unity Editor window select EditorNotesGPT. This creates a floating notes window.

Saved notes are stored in Editor preferences so your work is saved locally and private.

This comes with no warranty or promise that notes cannot be lost.

## Install

Package should now appear in package manager.

### via Git URL

Open `Packages/manifest.json` with your favorite text editor. Add following line to the dependencies block:
```json
{
  "dependencies": {
    "com.baawolf.EditorNotesGPT": "https://github.com/baawolf/EditorNotesGPT.git"
  }
}
```

## License

This prototype is not connected with OpenAI's ChatGPT in any way. The code has been released under the MIT License. 

Copyright © 2024 BaaWolf
