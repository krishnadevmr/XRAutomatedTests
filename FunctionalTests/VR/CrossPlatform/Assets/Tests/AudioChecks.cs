﻿using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UAssert = UnityEngine.Assertions.Assert;

public class AudioChecks : TestBaseSetup
{
    private AudioSource m_AudioSource = null;

    private readonly float audioPlayWait = 3f;
    private readonly float audioTolerance = .01f;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        m_TestSetupHelpers.TestCubeSetup(TestCubesConfig.TestCube);
        m_TestSetupHelpers.m_Cube.AddComponent<AudioSource>();
        m_TestSetupHelpers.m_Cube.GetComponent<AudioSource>().clip = Resources.Load("Audio/FOA_speech_ambiX", typeof(AudioClip)) as AudioClip;

        m_AudioSource = m_TestSetupHelpers.m_Cube.GetComponent<AudioSource>();
        m_TestSetupHelpers.m_Camera.AddComponent<AudioListener>();
    }

    [TearDown]
    public override void TearDown()
    {
        GameObject.Destroy(m_AudioSource);
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator AudioPlayCheck()
    {
        yield return null;

        m_AudioSource.Play();
        yield return new WaitForSeconds(audioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");
    }

    [UnityTest]
    public IEnumerator AudioSourceControlCheck()
    {
        yield return null;

        m_AudioSource.Play();
        yield return new WaitForSeconds(audioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        m_AudioSource.Pause();
        yield return new WaitForSeconds(audioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, false, "Audio source is not paused");

        m_AudioSource.UnPause();
        yield return new WaitForSeconds(audioPlayWait);
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source didn't un-paused");

        m_AudioSource.Stop();
        yield return null;
        Assert.AreEqual(m_AudioSource.isPlaying, false, "Audio failed to stop");
    }

    [UnityTest]
    public IEnumerator AudioSpatialize()
    {
        yield return null;
        m_AudioSource.spatialize = true;
        Assert.IsTrue(m_AudioSource.spatialize, "Spatialize has failed to turn on");
        Debug.Log("Enabling Spatialized Audio");

        m_AudioSource.Play();
        Debug.Log("Starting Audio");
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        var blendAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            blendAmount = blendAmount + 0.1f;
            m_AudioSource.spatialBlend = blendAmount;
            Debug.Log("Changing blend amount : " + blendAmount);

            yield return new WaitForSeconds(1f);

            UAssert.AreApproximatelyEqual(blendAmount, m_AudioSource.spatialBlend, audioTolerance, "Spatial Blend as failed to be set");
        }
    }

    [UnityTest]
    public IEnumerator AudioVolumeControl()
    {
        yield return null;

        m_AudioSource.Play();
        Debug.Log("Starting Audio");
        Assert.AreEqual(m_AudioSource.isPlaying, true, "Audio source is not playing");

        m_AudioSource.volume = 0f;
        Assert.AreEqual(0f, m_AudioSource.volume, "Volume was not set to 0;");

        yield return null;

        var volumeAmount = 0f;

        for (var i = 0f; i < 10f; ++i)
        {
            volumeAmount = volumeAmount + 0.1f;
            m_AudioSource.volume = volumeAmount;
            Debug.Log("Changing volume amount : " + volumeAmount);

            yield return new WaitForSeconds(1f);

            UAssert.AreApproximatelyEqual(volumeAmount, m_AudioSource.volume, "volume has failed to change to the desired level");
        }
    }
}
