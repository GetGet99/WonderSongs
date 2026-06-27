using System;
using System.Collections.Generic;
using Android.Media;
using Android.Util;

namespace WonderSongs.Droid;

public class AudioDeviceListener : AudioDeviceCallback
{
    /// <summary>
    /// Raised when a previously removed audio device has reconnected.
    /// This indicates the same physical device reappeared (for example, a flaky headphone plug
    /// that disconnected and then connected again). This event will only be raised if a prior
    /// removal was observed for a matching device id or device type.
    /// </summary>
    public event Action? SameDeviceConnected;

    /// <summary>
    /// Raised when an audio device is removed/disconnected.
    /// Subscribers can use this to detect that a device was disconnected.
    /// </summary>
    public event Action? DeviceDisconnected;

    // Track recently removed device ids and types to detect reconnects.
    private readonly HashSet<int> _removedDeviceIds = new();
    private readonly HashSet<AudioDeviceType> _removedDeviceTypes = new();
    private readonly object _lock = new();

    public override void OnAudioDevicesAdded(AudioDeviceInfo[]? addedDevices)
    {
        if (addedDevices == null || addedDevices.Length == 0)
            return;

        lock (_lock)
        {
            foreach (var device in addedDevices)
            {
                var id = device.Id;
                var type = device.Type;

                // If we observed a prior removal for the same id or type, consider this a reconnection.
                if (_removedDeviceIds.Contains(id) || _removedDeviceTypes.Contains(type))
                {
                    Log.Debug(nameof(AudioDeviceListener), $"Device reconnected: id={id}, type={type}");

                    // Remove from the removed sets to avoid duplicate SameDeviceConnected events.
                    _removedDeviceIds.Remove(id);
                    _removedDeviceTypes.Remove(type);

                    try
                    {
                        SameDeviceConnected?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn(nameof(AudioDeviceListener), $"Exception invoking SameDeviceConnected: {ex}");
                    }
                }
            }
        }
    }

    public override void OnAudioDevicesRemoved(AudioDeviceInfo[]? removedDevices)
    {
        if (removedDevices == null || removedDevices.Length == 0)
            return;

        lock (_lock)
        {
            foreach (var device in removedDevices)
            {
                var id = device.Id;
                var type = device.Type;

                _removedDeviceIds.Add(id);
                _removedDeviceTypes.Add(type);

                Log.Debug(nameof(AudioDeviceListener), $"Device removed: id={id}, type={type}");

                try
                {
                    DeviceDisconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    Log.Warn(nameof(AudioDeviceListener), $"Exception invoking DeviceDisconnected: {ex}");
                }
            }
        }
    }
}
