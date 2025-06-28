# AetherFM - Radio Streaming for Final Fantasy XIV

AetherFM is a Dalamud plugin that lets you listen to thousands of real radio stations directly in-game while playing Final Fantasy XIV!  
Modern UI, instant playback, and no setup required.

---

## 🎵 Features

- **Real-time Radio Streaming:** Listen to thousands of radio stations from around the world
- **Advanced Search & Filters:** Find stations by genre, country, or name
- **Modern UI:** Resizable window, searchable and filterable radio list
- **Custom URL:** Enter any custom stream URL
- **Audio Controls:** Play, Stop, and volume slider
- **Automatic Updates:** All stations loaded from the Radio Browser API (no local files needed)
- **Accessibility:** High-contrast, tooltips, and error feedback
- **Easy Dev/Repo Switch:** Seamless workflow for developers and users

---

## 📋 Requirements

- Final Fantasy XIV
- XIVLauncher with Dalamud
- .NET 7.0 or higher
- Internet connection for streaming

---

## 🛠️ Installation

1. **Add the repository to XIVLauncher:**
   - Go to `Settings → Experimental → Custom Plugin Repositories`
   - Add:  
     ```
     https://raw.githubusercontent.com/SalvatoreDevelopment/AetherFM/main/pluginmaster.json
     ```
2. **Search for “AetherFM”** in the plugin installer and click Install.

---

## 🎮 Usage

- **Open the radio player:**  
  `/aetherfm` or use the plugin button in-game.
- **Search/filter:**  
  Use the search bar and country filter to find your favorite station.
- **Play/Stop:**  
  Click the Play/Stop button next to any station.
- **Custom URL:**  
  Enter a direct stream URL if your station is not listed.
- **Volume:**  
  Adjust the volume slider at the top.

---

## 🚀 Development & Release Workflow

### Local Development (devPlugins)
- For rapid development and testing:
  ```powershell
  .\build.ps1 -DeployLocal
  ```
- This copies the plugin to `%APPDATA%\XIVLauncher\devPlugins\AetherFM`.
- XIVLauncher will always load this version if present, overriding any repository version.

### Test the Repository Version
- To test the plugin as a user would:
  ```powershell
  .\build.ps1 -SwitchToRepo
  ```
- This removes the local devPlugins version, so XIVLauncher loads the version from the repository.

### Create a Release ZIP
- To package the plugin for release:
  ```powershell
  .\build.ps1 -Zip
  ```
- Upload the resulting `AetherFM.zip` to your GitHub release and update your repository JSON.

### Best Practices
- **Never leave both devPlugins and repo versions active:** devPlugins always takes priority.
- **For fast iteration:** use `-DeployLocal` and test in-game.
- **For real user testing:** use `-SwitchToRepo` and install from repo.
- **For release:** use `-Zip`, upload, and update your repo JSON.
- **Always update the version in your repo JSON when releasing.**

---

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete version history.

---

## 📄 License

AGPL-3.0-or-later

---

## 🤝 Contributing

Contributions are welcome! Open an issue or pull request for:
- Bug reports
- Feature requests
- Documentation improvements
- Code optimizations

---

## 📞 Support

- **GitHub Issues:** [Open an issue](https://github.com/SalvatoreDevelopment/AetherFM/issues)
- **Discord:** Look for the #aetherfm channel in the Dalamud community
- **Email:** Contact the author via GitHub

---

**Happy listening in Eorzea!** 🎵✨
