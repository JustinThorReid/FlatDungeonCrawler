using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Frequency table https://pages.mtu.edu/~suits/notefreqs.html

public class Oscilator : MonoBehaviour {

    public Slider VolumeSlider;
    public Slider AttackSlider;
    public Slider DecaySlider;
    public Slider DecayTwoSlider;
    public Slider SustainSlider;
    public Slider ResonanceSlider;

    public float Frequency = 440f;
    private double Increment;
    private double Phase;
    private double SamplingFrequency = 48000.0;

    public float Gain;
    public float Volume = 0.1f;
    private float TargetVolume;
    private float StartFreq;

    public float[] Frequencies;

    public int ThisFreq;
    public int Octave = 4;

    public bool Played;
    public bool VolumeControl;

    void Start() {

        Frequencies = new float[108];//This is in the key of C major

        StartFreq = Mathf.Pow(2f, 1f / 12f);

        for(int i = 0; i < Frequencies.Length; i++)//calculates the frequencies for the notes
        {

            Frequencies[i] = (Mathf.Pow(StartFreq, i - 48)) * 440;

        }

    }

    void Update() {
        if(VolumeControl == true) {

            if(Input.GetKeyDown(KeyCode.Z))//Input
            {

                ThisFreq = 0 + (12 * Octave);
                Played = true;

            } else if(Input.GetKeyDown(KeyCode.S)) {

                ThisFreq = 1 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.X)) {

                ThisFreq = 2 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.D)) {

                ThisFreq = 3 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.C)) {

                ThisFreq = 4 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.V)) {

                ThisFreq = 5 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.G)) {

                ThisFreq = 6 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.B)) {

                ThisFreq = 7 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.H)) {

                ThisFreq = 8 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.N)) {

                ThisFreq = 9 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.J)) {

                ThisFreq = 10 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.M)) {

                ThisFreq = 11 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Comma)) {

                ThisFreq = 12 + (12 * Octave);
                Played = true;
            }
              //Up the keyboard
              else if(Input.GetKeyDown(KeyCode.Q)) {

                ThisFreq = 12 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha2)) {

                ThisFreq = 13 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.W)) {

                ThisFreq = 14 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha3)) {

                ThisFreq = 15 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.E)) {

                ThisFreq = 16 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.R)) {

                ThisFreq = 17 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha5)) {

                ThisFreq = 18 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.T)) {

                ThisFreq = 19 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha6)) {

                ThisFreq = 20 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Y)) {

                ThisFreq = 21 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha7)) {

                ThisFreq = 22 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.U)) {

                ThisFreq = 23 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.I)) {

                ThisFreq = 24 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha9)) {

                ThisFreq = 25 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.O)) {

                ThisFreq = 26 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.Alpha0)) {

                ThisFreq = 27 + (12 * Octave);
                Played = true;
            } else if(Input.GetKeyDown(KeyCode.P)) {

                ThisFreq = 28 + (12 * Octave);
                Played = true;
            }

            //if a valid key is not pressed
            if(!Input.anyKey) {
                Gain = 0f;
                Volume = 0f;
                Played = false;
            }
        }

        if(Played == true) {
            TargetVolume = VolumeSlider.value;
            if(Volume < TargetVolume) {


                Volume += AttackSlider.value / 3f;

            } else if(Volume > TargetVolume) {

                Volume = TargetVolume;

            }

            Gain = Volume;
            Frequency = Frequencies[ThisFreq];

        }

        if(Octave > 6) {

            Octave = 6;

        } else if(Octave < 0) {

            Octave = 0;

        }
    }

    //    +=======================================================================================================+
    //    |                                           Wave generator                                              |
    //    +=======================================================================================================+ 

    void OnAudioFilterRead(float[] data, int channels) {

        Increment = Frequency * 2 * Mathf.PI / SamplingFrequency;

        for(int i = 0; i < data.Length; i += channels) {

            Phase += Increment;
            data[i] = (float)(Gain * Mathf.Sin((float)Phase));

            if(channels == 2) {

                data[i + 1] = data[i];

            }

            if(Phase > Mathf.PI * 2) {

                Phase = 0.0;

            }

        }

    }

    //  +=======================================================================================================+
    //  |                                            Functions                                                  |
    //  +=======================================================================================================+ 

    public void StopPlaying() {

        VolumeControl = false;

    }

    public void StartPlaying() {

        VolumeControl = true;

    }

}