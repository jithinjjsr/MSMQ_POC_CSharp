# MSMQ Windows Service Setup and Troubleshooting

## Overview

This document outlines the steps to set up a Windows Service (using .NET Framework 4.5) that processes messages from one MSMQ queue and forwards them to another, including key troubleshooting and permission fixes.

---

## Setup Steps

1. **Create the Windows Service Project**
    - Use Visual Studio to create a Windows Service project targeting .NET Framework 4.5.
    - Name your service appropriately.

2. **Add Necessary References**
    - Add a reference to `System.Messaging` for MSMQ support.

3. **Service Logic**
    - The service should read messages from a source queue (`MyTestQueue`).
    - Modify each message and forward it to a destination queue (`MySecondTestQueue`). Create the destination queue if it does not exist.
    - Ensure queues are initialized on service start.

4. **Add Project Installer**
    - Use the service designer to add a ProjectInstaller for service registration.

5. **Configure the Installer**
    - Set the service name, display name, and description.
    - Set the service to start automatically.
    - Use the `Local System` account for development/testing.

6. **Build and Install the Service**
    - Build your solution (Debug/Release).
    - Use `installutil.exe` from the .NET Framework directory to install the service executable.
    - Start the service from Services MMC or via the `net start` command.

---

## Troubleshooting & Fixes

If the service runs but does not process messages and you observe `"Access to Message Queuing system is denied"` errors in Event Viewer, apply these fixes:

### 1. **Verify MSMQ Queue Permissions**
   - The service account should have *Full Control* permissions on both MSMQ queues.

### 2. **PowerShell Permission Fix (Recommended)**

Open PowerShell **as Administrator** and execute:

```powershell
# Stop the Windows Service and MSMQ service
Stop-Service -Name "MSMQMessageProcessorService" -Force
Stop-Service -Name "MSMQ" -Force

# Start MSMQ service
Start-Service -Name "MSMQ"

# Set Full Control for SYSTEM and Everyone on both queues
Get-MsmqQueue -Name "MyTestQueue" -QueueType Private | Set-MsmqQueueAcl -UserName "SYSTEM" -Allow FullControl
Get-MsmqQueue -Name "MyTestQueue" -QueueType Private | Set-MsmqQueueAcl -UserName "Everyone" -Allow FullControl
Get-MsmqQueue -Name "MySecondTestQueue" -QueueType Private | Set-MsmqQueueAcl -UserName "SYSTEM" -Allow FullControl
Get-MsmqQueue -Name "MySecondTestQueue" -QueueType Private | Set-MsmqQueueAcl -UserName "Everyone" -Allow FullControl

# Start the Windows Service again
Start-Service -Name "MSMQMessageProcessorService"
```

### 3. **Confirm Functionality**
   - Send a test message to `MyTestQueue`.
   - Confirm the service processes it and the message is forwarded to `MySecondTestQueue`.
   - Ensure no further "Access Denied" errors appear in Event Viewer.

---

## Notes

- When updating the code, stop the service, update the binaries, and then restart it. **You do not need to reinstall the service.**
- If you cannot set permissions with the GUI, always use the PowerShell method as described.
- Use Event Viewer to monitor all service errors and processing activity.

---

**End of Document**
