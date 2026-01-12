# GTA Key Automation Tool for PS4 / PS5 Console

This tool lets you **automate in-game actions** by replaying controller-style inputs.

Instead of pressing buttons manually, you create or use existing scripts that tell the program:

- which buttons to press  
- how long to wait  
- when to continue based on what the screen looks like  

The program then runs that script for you, consistently and automatically.


## What You Can Do With It

- Automate repeated in-game actions
- Trigger inputs only when a specific screen appears (screen detection / compare)
- Run the same sequence multiple times without manual input
- Use controller-style names (D-Pad, Cross, Circle, etc.) instead of raw keyboard keys

This is especially useful for tasks that require **timing, repetition, or waiting for menus/screens to appear**.




## Requirements

- A supported desktop operating system
- [Chiaki](https://github.com/streetpea/chiaki-ng/releases)/[Chiaki-ng](https://streetpea.github.io/chiaki-ng/) or another application that accepts keyboard input
- The target window must stay focused while automation is running
- Screen resolution and UI layout must match your image templates

---

## Important Notes

- Do not use the keyboard or mouse while a script is running
- Screen resolution or UI changes can break image matching
- Image templates must be BMP format
- This tool does **not** modify the game or console

---


---

# Chiaki or Chiaki-ng (PS Remote Play)

This tool is commonly used alongside **Chiaki / Chiaki-ng**, a third-party PlayStation Remote Play client. You must download Chiaki or Chiaki-ng in order to use the keyboard as controller input. The Sony Remote Play App **will not** support input like that. 

When used together:

- Chiaki streams the PlayStation to your computer
- This tool sends keyboard input to Chiaki
- The automation engine reacts to what appears on the streamed video feed

This allows automation of console gameplay **without modifying the console or the game itself**, so it is impossible to detect this specific application.

# Chiaki Setup Guide (Video)

Chiaki and/or Chiaki-ng are sometimes difficult to download when you have never done it before, so I created a YouTube tutorial on how to set it up properly to use with this rp.

### 📺 **YouTube Tutorial:**  

- [HarryHax Chiaki Tutorial](https://youtu.be/iadzYtX4ERU)




---

## How It Works (High Level)

1. You create a **script file** that defines a sequence of steps
2. Each step can:
   - Press a button
   - Wait for a period of time
   - Wait until the screen matches a reference image
3. The tool:
   - Sends keyboard input to the game (via Chiaki or directly)
   - Captures the screen
   - Compares it against saved image templates
4. Once all steps complete, the script ends

You do **not** need to write code.

---

## Writing Automation Scripts

Scripts are written in **JSON** and describe actions in plain language.

📺 **YouTube Tutorial:**  
[LINK COMING SOON]


---

## Input Mapping

Scripts use **controller-style names** (for example: D-Pad, Cross, Circle).

Internally, these are mapped to keyboard keys so the game or Chiaki receives input normally.

---

## Image Matching

- Capture a **BMP image** of the screen you want to detect
- The program compares the live screen to that image
- When the match percentage is high enough, the script continues



## Thank You

Thank you to everyone who has contributed ideas, testing, feedback, and support.

---

## Disclaimer

Use this tool responsibly and in accordance with the rules of the software or game you are automating.
