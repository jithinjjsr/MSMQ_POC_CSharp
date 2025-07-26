# MSMQ .NET Framework POC: Sender and Receiver (Steps Only)

## Overview
This document outlines the steps to create a simple proof of concept using Microsoft Message Queuing (MSMQ) with two .NET Framework console applications — one sending messages, the other receiving.

---

## Prerequisites

- Windows 11 Home or Pro with MSMQ enabled.
- Visual Studio installed with .NET Desktop Development workload.
- Target .NET Framework 4.7.2 or 4.8.

---

## Step 1: Enable MSMQ on Windows 11

- Open PowerShell as Administrator.
- Run the command to enable MSMQ server feature.
- Verify MSMQ service is running.

---

## Step 2: Setup Visual Studio Projects

- Create a new solution.
- Add two console app projects targeting .NET Framework 4.7.2 or 4.8.
  - Name one project `MSMQSender`.
  - Name the other `MSMQReceiver`.
- Add reference to the `System.Messaging` assembly in both projects.

---

## Step 3: Implement Sender Project

- Write code to:
  - Check if a private MSMQ queue exists; if not, create it.
  - Send a test message to the queue.
- Build and ensure no compilation errors.

---

## Step 4: Implement Receiver Project

- Write code to:
  - Connect to the same private MSMQ queue.
  - Receive and display messages from the queue.
- Build and ensure no compilation errors.

---

## Step 5: Run Both Projects Simultaneously

**Option A: Multi-Startup Projects in Visual Studio**

- Set the solution’s startup projects to run both `MSMQSender` and `MSMQReceiver`.
- Start debugging to open two console windows simultaneously.

**Option B: Run Executables Manually**

- Build the solution.
- Open two separate command prompt windows.
- Navigate to each project’s output folder.
- Run the sender and receiver executables separately.

---

## Expected Outcome

- The sender app creates the queue (if missing) and sends messages.
- The receiver app reads and displays those messages.
- This simulates inter-module communication via MSMQ, similar to legacy systems.

---

## Optional Enhancements

- Modify sender and receiver to process messages continuously in a loop.
- Use multiple queues for different modules or message types.
- Explore integration of MSMQ with native C++ components.
- Add logging, error handling, and configuration files for production readiness.

---

## References

- Microsoft official MSMQ documentation.
- .NET Framework `System.Messaging` namespace.
- Windows MSMQ installation guides.

---

*End of steps for MSMQ .NET Framework POC.*
