// Copyright 2017 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerDeviceInterface : componentInterface
{

    public string vidFilename;
    public Renderer videoSurface;
    public GameObject vidQuad;
    AudioSource source;
    //MovieTexture movieTexture;
    VideoPlayer videoPlayer;
    VideoPlayerUI vidUI;

    bool loading = false;
    bool loaded = false;
    public bool playing = false;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        vidUI = GetComponentInChildren<VideoPlayerUI>();
    }

    tooltips _tooltip;
    public void Autoplay(string file, tooltips _t)
    {
        videoSurface.material.SetColor("_TintColor", new Color32(0x9A, 0x9A, 0x9A, 0x80));
        _tooltip = _t;
        vidFilename = file;
        togglePlay();
        vidUI.updateControlQuad();
        vidUI.controlQuad.SetActive(false);
        vidUI.controlRends[0].material.SetFloat("_EmissionGain", 0f);
    }

    void Update()
    {
        if (loaded && playing)
        {
            if (!videoPlayer.isPlaying)
            {
                endPlayback();
            }
        }
    }

    void endPlayback()
    {
        playing = false;
        videoPlayer.Stop();
        vidQuad.SetActive(false);
        vidUI.Reset();
        masterControl.instance.toggleInstrumentVolume(true);
        if (_tooltip != null) _tooltip.ToggleVideo(false);
    }

    public void togglePlay()
    {
        playing = !playing;
        if (playing)
        {
            if (loaded)
            {
                vidQuad.SetActive(true);
                videoPlayer.Play();
                source.Play();
                masterControl.instance.toggleInstrumentVolume(false);
            }
            else if (!loading) StartCoroutine(movieRoutine());
        }
        else if (loaded)
        {
            videoPlayer.Pause();
            masterControl.instance.toggleInstrumentVolume(true);
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            endPlayback();
            videoSurface.material.mainTexture = null;
            videoPlayer = null;
        }
        loading = false;
        loaded = false;
        playing = false;
        masterControl.instance.toggleInstrumentVolume(true);
    }

    IEnumerator movieRoutine()
    {
        loading = true;

        videoPlayer.url = "file:///" + Application.streamingAssetsPath + System.IO.Path.DirectorySeparatorChar + vidFilename;
        
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.targetMaterialRenderer = videoSurface;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, source);

        if (playing)
        {
            vidQuad.SetActive(true);

            videoPlayer.Play();
            source.Play();
            masterControl.instance.toggleInstrumentVolume(false);
        }
        else
        {
            vidQuad.SetActive(false);
        }
        loading = false;
        loaded = true;
    }
}
