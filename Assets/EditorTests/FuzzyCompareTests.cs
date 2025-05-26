using NUnit.Framework;
using UnityEngine;

public class FuzzyCompareTests
{
    [Test]
    public void FuzzyEquals0_Float_Zero_ReturnsTrue()
    {
        float value = 0f;
        Assert.IsTrue(value.FuzzyEquals0());
    }

    [Test]
    public void FuzzyEquals0_Float_NearZeroWithinEpsilon_ReturnsTrue()
    {
        float value = 1e-8f;
        Assert.IsTrue(value.FuzzyEquals0(1e-7));
    }

    [Test]
    public void FuzzyEquals0_Float_OutsideEpsilon_ReturnsFalse()
    {
        float value = 0.01f;
        Assert.IsFalse(value.FuzzyEquals0(1e-4));
    }

    [Test]
    public void FuzzyEquals_Float_EqualValues_ReturnsTrue()
    {
        float a = 1.0f, b = 1.0f;
        Assert.IsTrue(a.FuzzyEquals(b));
    }

    [Test]
    public void FuzzyEquals_Float_DifferentWithinEpsilon_ReturnsTrue()
    {
        float a = 1.0f, b = 1.000001f;
        Assert.IsTrue(a.FuzzyEquals(b, 1e-5));
    }

    [Test]
    public void FuzzyEquals_Float_DifferentOutsideEpsilon_ReturnsFalse()
    {
        float a = 1.0f, b = 1.1f;
        Assert.IsFalse(a.FuzzyEquals(b, 1e-3));
    }

    [Test]
    public void FuzzyEquals_Vector2_EqualVectors_ReturnsTrue()
    {
        Vector2 a = new Vector2(1, 2);
        Vector2 b = new Vector2(1, 2);
        Assert.IsTrue(a.FuzzyEquals(b));
    }

    [Test]
    public void FuzzyEquals_Vector2_NearVectorsWithinEpsilon_ReturnsTrue()
    {
        Vector2 a = new Vector2(1, 2);
        Vector2 b = new Vector2(1.000001f, 2.000001f);
        Assert.IsTrue(a.FuzzyEquals(b, 1e-5));
    }

    [Test]
    public void FuzzyEquals_Vector2_DifferentVectorsOutsideEpsilon_ReturnsFalse()
    {
        Vector2 a = new Vector2(1, 2);
        Vector2 b = new Vector2(2, 3);
        Assert.IsFalse(a.FuzzyEquals(b, 1e-4));
    }

    [Test]
    public void FuzzyEquals0_Vector2_Zero_ReturnsTrue()
    {
        Vector2 a = Vector2.zero;
        Assert.IsTrue(a.FuzzyEquals0());
    }

    [Test]
    public void FuzzyEquals_Vector3_EqualVectors_ReturnsTrue()
    {
        Vector3 a = new Vector3(1, 2, 3);
        Vector3 b = new Vector3(1, 2, 3);
        Assert.IsTrue(a.FuzzyEquals(b));
    }

    [Test]
    public void FuzzyEquals_Vector3_NearVectorsWithinEpsilon_ReturnsTrue()
    {
        Vector3 a = new Vector3(1, 2, 3);
        Vector3 b = new Vector3(1.000001f, 2.000001f, 3.000001f);
        Assert.IsTrue(a.FuzzyEquals(b, 1e-5));
    }

    [Test]
    public void FuzzyEquals_Vector3_DifferentVectorsOutsideEpsilon_ReturnsFalse()
    {
        Vector3 a = new Vector3(1, 2, 3);
        Vector3 b = new Vector3(2, 3, 4);
        Assert.IsFalse(a.FuzzyEquals(b, 1e-4));
    }

    [Test]
    public void FuzzyEquals0_Vector3_Zero_ReturnsTrue()
    {
        Vector3 a = Vector3.zero;
        Assert.IsTrue(a.FuzzyEquals0());
    }

    [Test]
    public void IsValidViewingVector_Zero_ReturnsFalse()
    {
        Vector3 a = Vector3.zero;
        Assert.IsFalse(a.IsValidViewingVector());
    }

    [Test]
    public void IsValidViewingVector_NonZero_ReturnsTrue()
    {
        Vector3 a = new Vector3(1, 0, 0);
        Assert.IsTrue(a.IsValidViewingVector());
    }
}
