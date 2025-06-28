# AetherFM - Radio Streaming for Final Fantasy XIV

A Dalamud plugin that lets you listen to radio directly in-game while playing Final Fantasy XIV!

## 🎵 Features

- **Real-time Radio Streaming**: Listen to thousands of radio stations from around the world
- **Radiosure Database**: Automatic access to over 30,000 radio stations
- **Advanced Search**: Find stations by genre, country, or name
- **Custom URL**: Enter any custom stream URL
- **Automatic Updates**: Download the latest station database with one click
- **Audio Controls**: Play, Stop and playback state management
- **Integrated Interface**: Native window that integrates seamlessly with FFXIV

## 📋 Requirements

- Final Fantasy XIV
- XIVLauncher with Dalamud
- .NET 7.0 or higher
- Internet connection for streaming

## 🛠️ Installation

1. Download the plugin `.dll` file from the Releases section
2. Copy it to the folder `%APPDATA%\XIVLauncher\addon\Hooks\dev\Plugins\`
3. Restart XIVLauncher
4. Enable the plugin in-game via `/xlplugins`

## 🎮 Usage

### Commands
- `/aetherfm` - Opens the main radio player window
- `/xlplugins` → AetherFM → Settings - Access settings

### Interface
1. **Station Selection**: Use the dropdown menu to choose from thousands of stations
2. **Search**: Type to filter stations by name or genre
3. **Manual URL**: Enter a custom stream URL directly
4. **Controls**: Use Play/Stop buttons to control playback
5. **Update Stations**: Click "Update stations" to download the latest database

## 🎵 Available Stations

The plugin includes access to:
- **Italian Radio**: RAI, Radio Deejay, Radio 105, and many others
- **International Radio**: BBC, NPR, Radio France, and over 30,000 stations
- **Music Genres**: Rock, Pop, Jazz, Classical, EDM, and much more
- **Custom Streams**: Support for any audio stream URL

## 🔧 Development

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 7.0 SDK
- XIVLauncher with Dalamud in development mode

### Build
```bash
dotnet build
```

### Debug
1. Launch FFXIV with XIVLauncher in development mode
2. Attach debugger to the `ffxiv_dx11.exe` process
3. The plugin will automatically reload on each build

## 📦 Dependencies

- **NAudio**: For audio playback and stream management
- **Dalamud**: FFXIV plugin framework
- **ImGui.NET**: For user interface

## 🐛 Troubleshooting

### Audio not working
- Verify that system volume is active
- Check that no other programs are blocking audio
- Restart the plugin with `/xlplugins` → AetherFM → Reload

### Stations not loading
- Click "Update stations" to download the updated database
- Verify internet connection
- Check that firewall is not blocking connections

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete version history.

## 📄 License

AGPL-3.0-or-later

## 🤝 Contributing

Contributions are welcome! Open an issue or pull request for:
- Bug reports
- Feature requests
- Documentation improvements
- Code optimizations

## 📞 Support

- **GitHub Issues**: [Open an issue](https://github.com/SalvatoreDevelopment/AetherFM/issues)
- **Discord**: Look for the #aetherfm channel in the Dalamud community
- **Email**: Contact the author via GitHub

## ⭐ Stars

If you like AetherFM, consider giving a star to the repository! ⭐

---

**Happy listening in Eorzea!** 🎵✨ 