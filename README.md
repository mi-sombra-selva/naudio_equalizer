# NAudio Equalizer

A modern audio equalizer application built with C#, NAudio, and WinUI for real-time sound processing and precise frequency control.

## Features
- Real-time audio playback with progress tracking
- Support for MP3 and WAV formats
- Drag-and-drop file support
- 10-band equalizer with frequency labels (60Hz - 16kHz)
- Full playback controls (Play, Pause, Stop)
- Volume control with visual feedback
- Progress bar with time display
- Modern dark theme interface
- Tooltips for all controls
- Real-time equalizer processing
- Optimized audio processing with proper thread safety
- Standardized code style and documentation
- MVVM architecture implementation
- Visual equalizer representation with gradient bars

## Technologies
- **C#** — Core programming language
- **NAudio** (v2.2.1) — Audio processing library
- **WinUI** (v1.7.250310001) — UI framework for Windows applications
- **.NET 8.0** — Target framework

## Current Status
The application currently supports:
- Loading and playing audio files with progress tracking
- Complete playback controls with time display
- 10-band equalizer with frequency labels
- Independent volume control
- Modern dark theme UI
- Enhanced error handling and null safety
- Improved resource management
- Real-time equalizer processing
- Optimized audio processing
- Standardized code style and documentation
- MVVM architecture with service interfaces
- Visual equalizer representation with gradient bars

## Roadmap
- [x] Basic UI layout
- [x] Audio playback with NAudio
- [x] File loading and drag-and-drop
- [x] Progress bar implementation
- [x] Volume control
- [x] 10-band equalizer interface
- [x] Frequency band labels
- [x] Real-time equalizer processing
- [x] Code style standardization
- [x] MVVM architecture implementation
- [x] Visual equalizer representation
- [ ] Unit testing setup
- [ ] Preset management system
- [ ] Spectrum visualization
- [ ] Audio metadata display
- [ ] Keyboard shortcuts
- [ ] User documentation
- [ ] Advanced DSP features
- [ ] Equalizer presets
- [ ] Additional audio format support
- [ ] Integration tests
- [ ] UI tests
- [ ] Performance tests
- [ ] Automated build pipeline

## Requirements
- Windows 10 (version 10.0.17763.0 or higher)
- .NET 8.0 SDK
- Visual Studio 2022 or later (recommended)

## Getting Started
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Build and run the project
4. Use the "Select File" button or drag-and-drop to load an audio file
5. Use the equalizer sliders to adjust different frequency bands
6. Control volume with the dedicated slider on the left
7. Use the progress bar for precise playback position control
8. Use the Reset button to restore default equalizer settings
9. Watch the visual equalizer representation for real-time feedback

## Development
### Building
```bash
dotnet build
```

### Code Style
- All code comments and UI elements are in English
- Follows standard C# coding conventions
- Uses proper indentation and formatting
- Implements comprehensive error handling
- Follows MVVM pattern best practices

## License
[MIT](LICENSE)