# Modern Pomodoro Timer 🍅

<img width="688" height="689" alt="Screenshot" src="https://github.com/user-attachments/assets/95f1e3c7-42c2-42f6-bed2-9ed5a040ceb2" />

A sleek, modern, and distraction-free Pomodoro Timer built with **.NET 10** and **WPF**. 
Designed to help you focus on work and manage breaks efficiently, wrapped in a beautiful dark-themed UI.

## 🚀 Features

### 🎨 User Interface
* **Deep Dark Theme:** Easy on the eyes, perfect for night coding sessions.
* **Custom Window:** A borderless, 700x700 window with custom maximize/close controls.
* **Vector Icons:** Sharp visuals on any display resolution.
* **Smooth Animations:** Cross-fade transitions when switching between Timer and Configuration modes.

### ⚡ User Experience
* **Smart Controls:** * Large, ergonomic Play/Pause button.
    * Quick time adjustments (+/-) directly on the card.
    * Configuration applies instantly (live update).
* **Taskbar Integration:** Track your session progress directly on the taskbar icon without opening the window.
* **Custom Dialogs:** No more ugly system message boxes. Confirmation dialogs are built into the UI as aesthetic overlays.
* **History Tracking:** Review your recent sessions in a clean, 2-column grid layout.

### 🛠 Architecture
* **MVVM Pattern:** Built using `CommunityToolkit.Mvvm` for a clean, testable codebase.
* **Modern .NET:** Runs on .NET 10 for maximum performance.
* **Async I/O:** Non-blocking JSON data persistence for session history.

## 📦 Installation

1.  Go to the [Releases](https://github.com/enviGit/PomodoroTimer/releases/latest) page.
2.  Download the latest `PomodoroTimer.zip`.
3.  Extract and run `PomodoroTimer.exe`.

## 📂 Project Structure

* **`ViewModels/`**: Contains the presentation logic (`MainViewModel`).
* **`Models/`**: Data structures (immutable `Session` records).
* **`Services/`**: Handles data persistence (`SessionService`).
* **`Core/`**: The heart of the timer logic (`PomodoroEngine`).
* **`Converters/`**: XAML value converters for dynamic UI updates.
* **`Resources/`**: Icons.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
