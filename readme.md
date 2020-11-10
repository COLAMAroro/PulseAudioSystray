# PulseAudioSystray

I was annoyed when running Linux apps on the WSL. I have X410, with a systray icon, so I can close nice and simple

But not PulseAudio. I have to use the task manager to close it. So I made this small wrapper to do it for me

## How to set it up

Install PulseAudio in C:\PulseAudio or somewhere else (in this case, set the `pulseaudiopath` environment variable)

Build it yourself or just get the release, place it wherever, run it.