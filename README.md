# 🧩 Quantum Puzzle Lab

Ein 3D Puzzle-Spiel mit physikalischen Unlogischkeiten, inspiriert von Superliminal.

## 🎮 Features

### **Kernmechaniken:**
- **Superliminal-Style Greifen** - Objekte greifen und durch Perspektive skalieren
- **Druckplatten-System** - Gewichtsbasierte Aktivierung von Türen
- **Interaktions-System** - Schalter, Türen, Kristalle sammeln
- **Physik-Zonen** - Anti-Gravity, Time Distortion, Gravity Rotation

### **Player Controller:**
- **Smooth Movement** - WASD + Maus-Steuerung
- **Head-Bob** - Realistische Lauf-Animation
- **FOV-Effekte** - Dynamische Sichtfeld-Änderungen
- **Sprint-System** - Schnellere Bewegung mit visuellen Effekten

### **Audio-System:**
- **Schrittgeräusche** - Unterschiedliche Geschwindigkeiten
- **Interaktions-Sounds** - Für alle Aktionen
- **Hintergrundmusik** - Atmospheric Audio
- **3D Audio** - Räumliche Geräusche

### **Post-Processing:**
- **Color Correction** - Helligkeit, Kontrast, Sättigung
- **Vignette** - Dunkle Ränder-Effekt
- **Motion Blur** - Bewegungsunschärfe

### **Save/Load System:**
- **Einstellungen** - Maus-Sensitivity, Audio, Grafik
- **Fortschritt** - Level-Zeiten, Kristalle, freigeschaltete Level
- **JSON-basiert** - Cross-Platform kompatibel

## 🚀 Installation

1. **Unity 2022.3+** herunterladen
2. **Repository klonen:**
   ```bash
   git clone https://github.com/dein-username/quantum-puzzle-lab.git
   cd quantum-puzzle-lab
   ```
3. **Projekt in Unity öffnen**
4. **Szene laden:** `Assets/Scenes/QuantumPuzzleLab.unity`
5. **Play drücken** - Alles wird automatisch eingerichtet!

## 🎯 Steuerung

| Taste | Aktion |
|-------|--------|
| **WASD** | Bewegung |
| **Maus** | Kamera umschauen |
| **Leertaste** | Springen |
| **Links Shift** | Sprinten |
| **E** | Interagieren/Greifen |
| **Mausrad** | Objekt-Distanz ändern |
| **Rechtsklick** | Objekt werfen |
| **ESC** | Cursor freigeben |

## 🧩 Puzzle-Elemente

### **Druckplatten:**
- **Gewichts-Erkennung** - Player und Objekte haben unterschiedliche Gewichte
- **Visuelle Effekte** - Platte sinkt ein, Farbe ändert sich
- **Audio-Feedback** - Aktivierungs- und Deaktivierungs-Sounds
- **Tür-Verbindung** - Automatische Tür-Steuerung

### **Physik-Zonen:**
- **Anti-Gravity** - Objekte schweben nach oben
- **Time Distortion** - Zeit verlangsamt sich
- **Gravity Rotation** - Schwerkraft dreht sich
- **Quantum Superposition** - Objekte teleportieren zwischen Positionen
- **Phase Shifting** - Objekte werden durchlässig

### **Interaktive Objekte:**
- **Schalter** - Rotieren und lösen Events aus
- **Türen** - Öffnen/schließen mit Animation
- **Kristalle** - Sammelbare Objekte für Fortschritt
- **Grabbare Objekte** - Physik-basierte Manipulation

## 🛠️ Technische Details

### **Architektur:**
- **Modularer Aufbau** - Jede Mechanik ist ein separates Script
- **Event-System** - Lose gekoppelte Komponenten
- **Singleton-Pattern** - Für Manager-Klassen
- **Component-Based** - Wiederverwendbare Scripts

### **Performance:**
- **Object Pooling** - Für häufig verwendete Objekte
- **LOD-System** - Level of Detail für große Szenen
- **Culling** - Frustum und Occlusion Culling
- **Batching** - Static und Dynamic Batching

### **Skalierbarkeit:**
- **Level-System** - Einfaches Hinzufügen neuer Level
- **Mod-Support** - ScriptableObject-basierte Konfiguration
- **Multiplayer-Ready** - Netzwerk-kompatible Architektur

## 📁 Projektstruktur

```
Assets/
├── Scripts/
│   ├── Player/           # Player Controller, Camera, Movement
│   ├── Interaction/      # Greifen, Interaktionen, UI
│   ├── Physics/          # Physik-Zonen, Druckplatten
│   ├── Audio/            # Audio Manager, Sound Effects
│   ├── UI/               # UI Manager, Menüs, HUD
│   ├── Save/             # Save System, Settings
│   └── Level/            # Level Manager, Scene Loading
├── Shaders/              # Custom Shaders für Post-Processing
├── Scenes/               # Game Scenes
└── Materials/            # Game Materials
```

## 🎨 Level-Design

### **Aktuelle Level:**
- **Tutorial-Bereich** - Einführung in die Mechaniken
- **Druckplatten-Rätsel** - Gewichtsbasierte Herausforderungen
- **Physik-Zonen** - Experimentelle Bereiche
- **Kristall-Sammlung** - Fortschritts-Tracking

### **Geplante Erweiterungen:**
- **Laser-System** - Licht-basierte Rätsel
- **Teleporter** - Portal-Mechaniken
- **Zeit-Rätsel** - Chronologische Herausforderungen
- **Multi-Player** - Kooperative Rätsel

## 🤝 Beitragen

1. **Fork** das Repository
2. **Feature Branch** erstellen: `git checkout -b feature/neue-funktion`
3. **Änderungen committen:** `git commit -m 'Add neue Funktion'`
4. **Branch pushen:** `git push origin feature/neue-funktion`
5. **Pull Request** erstellen

## 📝 Lizenz

Dieses Projekt steht unter der MIT-Lizenz. Siehe [LICENSE](LICENSE) für Details.

## 🙏 Danksagungen

- **Superliminal** - Inspiration für die Perspektiv-Mechanik
- **Unity Community** - Für Tutorials und Support
- **Open Source** - Für die verwendeten Libraries

## 📞 Kontakt

- **GitHub:** [@dein-username](https://github.com/dein-username)
- **Email:** deine-email@example.com
- **Discord:** dein-discord#1234

---

**Viel Spaß beim Lösen der Quantum-Puzzles!** 🧩✨