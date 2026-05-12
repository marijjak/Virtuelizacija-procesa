# 🌦️ Meteorological Station Monitoring System  
### *WCF-Based Weather Data Streaming & Analytics Platform*

![C#](https://img.shields.io/badge/C%23-.NET-blue?style=for-the-badge&logo=csharp)
![WCF](https://img.shields.io/badge/WCF-Service-green?style=for-the-badge)
![CSV](https://img.shields.io/badge/Data-CSV-orange?style=for-the-badge)
![Status](https://img.shields.io/badge/Project-Academic-success?style=for-the-badge)
![Architecture](https://img.shields.io/badge/Architecture-Client%20%E2%86%94%20Server-purple?style=for-the-badge)

---

# 📌 Project Overview

The **Meteorological Station Monitoring System** is a distributed C# application designed for **real-time weather data transmission, validation, storage, and analysis** using **WCF services**, **streaming communication**, and an **event-driven architecture**.

The project simulates a meteorological station that continuously sends atmospheric measurements to a central server for processing and anomaly detection.

The system supports:

✅ Real-time streaming of weather measurements  
✅ CSV dataset parsing and processing  
✅ WCF communication via `netTcpBinding`  
✅ Event-driven alerts and notifications  
✅ Detection of abnormal meteorological changes  
✅ Disk-based storage and logging  
✅ Resource-safe stream handling using `IDisposable`

---

# 🏗️ System Architecture

```text
+-------------------+
|   CLIENT APP      |
| Meteorological    |
| Station Simulator |
+---------+---------+
          |
          | StartSession()
          | PushSample()
          | EndSession()
          v
+-------------------+
|    WCF SERVICE    |
| Validation Layer  |
| Event Processing  |
| Analytics Engine  |
+---------+---------+
          |
          v
+-------------------+
| FILE STORAGE      |
| measurements.csv  |
| rejects.csv       |
| session_log.txt   |
+-------------------+
```

---

# ⚙️ Technologies Used

| Technology | Purpose |
|---|---|
| **C# (.NET Framework)** | Core application development |
| **WCF (Windows Communication Foundation)** | Client-server communication |
| **netTcpBinding** | High-performance TCP transport |
| **CSV Parsing** | Dataset loading |
| **FileStream / StreamWriter** | File storage |
| **Delegates & Events** | Event-driven notifications |
| **LINQ** | Data processing |
| **IDisposable Pattern** | Resource management |

---

# 📂 Project Structure

```text
MeteorologicalStation/
│
├── Client/
│   ├── CSV Reader
│   ├── Session Sender
│   └── Streaming Logic
│
├── Server/
│   ├── WCF Service
│   ├── Validation
│   ├── Analytics
│   ├── Event Handlers
│   └── Storage Management
│
├── Shared/
│   ├── Data Contracts
│   ├── Models
│   └── Fault Contracts
│
├── Data/
│   └── weather_dataset.csv
│
└── Logs/
    ├── measurements_session.csv
    ├── rejects.csv
    └── session_log.txt
```

---

# 📡 Communication Protocol

## Session Workflow

### 1️⃣ Start Session

Client initializes transmission:

```csharp
StartSession(meta)
```

### 2️⃣ Stream Samples

Client sequentially sends rows from CSV:

```csharp
PushSample(sample)
```

### 3️⃣ End Session

Transmission is finalized:

```csharp
EndSession()
```

---

# 📑 Session Metadata

Each session contains:

```csharp
{
    T,
    Pressure,
    Tpot,
    Tdew,
    Rh,
    Sh,
    Date
}
```

| Field | Description |
|---|---|
| T | Air temperature |
| Pressure | Atmospheric pressure |
| Tpot | Potential temperature |
| Tdew | Dew point |
| Rh | Relative humidity |
| Sh | Solar radiation |
| Date | Measurement timestamp |

---

# 🔍 Data Validation

The server validates:

- Required fields
- Data types
- Numeric ranges
- Measurement units
- Null values

Example:

```csharp
if (RelativeHumidity <= 0)
{
    throw new ValidationFault("Invalid humidity value");
}
```

---

# 🚨 Event-Driven Detection System

The application automatically detects abnormal atmospheric behavior.

## 🌡️ Temperature Spike Detection

Formula:

```text
ΔT = T[n] - T[n-1]
```

Condition:

```text
|ΔT| > T_threshold
```

Event triggered:

```csharp
TemperatureSpike
```

---

## 💧 Humidity Spike Detection

Formula:

```text
ΔRH = RH[n] - RH[n-1]
```

Condition:

```text
|ΔRH| > RH_threshold
```

Event triggered:

```csharp
RHSpike
```

---

## 🌫️ Dew Point Spike Detection

Formula:

```text
ΔTdew = Tdew[n] - Tdew[n-1]
```

Condition:

```text
|ΔTdew| > DEW_threshold
```

Event triggered:

```csharp
DEWSpike
```

---

# 📊 Running Mean Monitoring

The server continuously calculates:

```text
Tmean
```

Out-of-band warning:

```text
T < 0.75 * Tmean
OR
T > 1.25 * Tmean
```

Triggered event:

```csharp
OutOfBandWarning
```

---

# 🔔 Implemented Events

| Event | Description |
|---|---|
| OnTransferStarted | Session started |
| OnSampleReceived | Sample received |
| OnTransferCompleted | Transfer completed |
| OnWarningRaised | Warning generated |
| TemperatureSpike | Sudden temperature change |
| RHSpike | Sudden humidity change |
| DEWSpike | Sudden dew point change |

---

# 💾 File Management

The server automatically creates:

## measurements_session.csv

Stores valid measurements.

## rejects.csv

Stores invalid/rejected samples.

## session_log.txt

Stores:

- Session information
- Warnings
- Errors
- Events
- Statistics

---

# 🧹 Resource Management

The project properly implements:

```csharp
IDisposable
```

for:

- Streams
- Readers
- Writers
- File handlers

Example:

```csharp
using(StreamWriter writer = new StreamWriter(path))
{
    writer.WriteLine(data);
}
```

---

# ⚡ WCF Configuration

Example configuration:

```xml
<netTcpBinding>
    <binding
        transferMode="Streamed"
        maxReceivedMessageSize="10485760"
        sendTimeout="00:10:00"
        receiveTimeout="00:10:00"/>
</netTcpBinding>
```

---

# 📥 Dataset

Dataset source:

🌍 Kaggle Weather Dataset

The client loads:

✅ First 100 rows  
✅ Invariant culture parsing  
✅ Decimal point formatting  
✅ Invalid rows logged separately

---

# ▶️ How to Run

## 1️⃣ Start the WCF Server

```bash
Server.exe
```

## 2️⃣ Start the Client

```bash
Client.exe
```

## 3️⃣ Observe Streaming

Console output:

```text
Transfer in progress...
Sample received...
Transfer completed.
```

---

# 🧪 Example Console Output

```text
[INFO] Session started
[INFO] Sample received
[WARNING] Temperature spike detected
[WARNING] Humidity spike detected
[INFO] Transfer completed
```

---

# 🎯 Project Goals

The main objectives of the project are:

- Simulate real-time weather monitoring
- Practice WCF service development
- Implement event-driven programming
- Learn streaming and file handling
- Detect environmental anomalies
- Apply clean resource management

---

# 📚 Academic Concepts Covered

✅ WCF Services  
✅ Data Contracts  
✅ Service Contracts  
✅ TCP Streaming  
✅ File Handling  
✅ IDisposable Pattern  
✅ Delegates & Events  
✅ Exception Handling  
✅ CSV Parsing  
✅ Real-Time Analytics

---

# 👩‍💻 Authors

### 👩 Marija Krsmanovic  
### 👩 Gordana Petrović

---

# 🔗 Repository

https://github.com/marijjak/Virtuelizacija-procesa.git

---

# ⭐ Final Notes

This project demonstrates a complete distributed weather-monitoring solution built with modern C# concepts and service-oriented architecture principles. It combines networking, streaming, analytics, event processing, and resource management into a fully functional meteorological monitoring platform.
