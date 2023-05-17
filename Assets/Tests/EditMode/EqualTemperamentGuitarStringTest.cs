using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class EqualTemperamentGuitarStringTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void constructor_initializes_number_of_frets()
    {
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(20);
        Assert.AreEqual(20, equalTemperamentGuitarString.GetNumberOfFrets());
    }


    [Test]
    [TestCase(0.05f, 1)]
    [TestCase(0.19f, 2)]
    [TestCase(0.31f, 4)]
    [TestCase(0.53f, 6)]
    [TestCase(0.96f, 10)]
    public void holding_at_position_sets_corresponding_frets(float position, int expectedFret)
    {
        //Arrange
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(10);

        //Act
        equalTemperamentGuitarString.HoldAt(position);
        int actualFretSelected = equalTemperamentGuitarString.GetSelectedFret();

        //Assert
        Assert.AreEqual(expectedFret, actualFretSelected);
    }

    [Test]
    [TestCase(1.2f, 10)]
    [TestCase(-0.1f, 0)]
    public void fret_selection_handles_exception_in_ranges(float position, int expectedFret)
    {
        //Arrange
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(10);

        //Act
        equalTemperamentGuitarString.HoldAt(position);
        int actualFretSelected = equalTemperamentGuitarString.GetSelectedFret();

        //Assert
        Assert.AreEqual(expectedFret, actualFretSelected);
    }


    [Test]
    [TestCase(0, 1f)]
    [TestCase(1, 1.059463f)]
    [TestCase(4, 1.259921f)]
    [TestCase(9, 1.681793f)]
    [TestCase(12, 2f)]
    public void setting_fret_sets_corresponding_pitch(int fretSelected, float expectedPitch)
    {//equal temeprament must follow equal increment in frequency and correspondingly pitch
        //Arrange
        float toleranceLevel = 0.0001f;
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(12);

        //Act
        equalTemperamentGuitarString.SetFret(fretSelected);
        float actualPitch = equalTemperamentGuitarString.GetPitch();

        //Assert
        Assert.That(expectedPitch, Is.EqualTo(actualPitch).Using(new FloatEqualityComparer(toleranceLevel)) );
    }


    [Test]
    public void resetting_hold_resets_pitch_to_one()
    {//bent and held string is reset back to open position meaning pitch becomes 1.0 again
        //Arrange
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(12);

        //Act
        equalTemperamentGuitarString.HoldAt(0.25f);
        equalTemperamentGuitarString.Bend(0.5f);
        equalTemperamentGuitarString.ResetHold();

        //Assert
        Assert.AreEqual(equalTemperamentGuitarString.GetPitch(), 1.0f);
    }


    [Test]
    [TestCase(0f, 1f)]
    [TestCase(0.5f, 1.029732f)]
    [TestCase(1.0f, 1.059463f)]
    [TestCase(2.0f, 1.118926f)]
    public void bending_with_open_string_sets_corresponding_pitch(float bendAmount, float expectedPitch)
    {//equal temeprament must follow equal increment in frequency and correspondingly pitch
        //Arrange
        float toleranceLevel = 0.0001f;
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(12);

        //Act
        equalTemperamentGuitarString.Bend(bendAmount);
        float actualPitch = equalTemperamentGuitarString.GetPitch();

        //Assert
        Assert.That(expectedPitch, Is.EqualTo(actualPitch).Using(new FloatEqualityComparer(toleranceLevel)));
    }

    [Test]
    [TestCase(0.0f, 0.20f, 1.122462f)]
    [TestCase(0.5f, 0.81f, 1.731795f)]
    [TestCase(1.0f, 0.99f, 1.887749f)]
    [TestCase(2.0f, 0.42f, 1.493587f)]
    public void bending_while_holding_string_sets_corresponding_pitch(float bendAmount, float holdPosition, float expectedPitch)
    {//equal temeprament must follow equal increment in frequency and correspondingly pitch
        //Arrange
        float toleranceLevel = 0.0001f;
        EqualTemperamentGuitarString equalTemperamentGuitarString = new EqualTemperamentGuitarString(10);

        //Act
        equalTemperamentGuitarString.HoldAt(holdPosition);
        equalTemperamentGuitarString.Bend(bendAmount);
        float actualPitch = equalTemperamentGuitarString.GetPitch();

        //Assert
        Assert.That(expectedPitch, Is.EqualTo(actualPitch).Using(new FloatEqualityComparer(toleranceLevel)));
    }


}
