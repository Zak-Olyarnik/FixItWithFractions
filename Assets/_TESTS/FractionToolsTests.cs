using NUnit.Framework;

public class FractionToolsTests
{
    [Test]
    public void AtomizeFractionLargestChunkTest()
    {
        FractionTools.Fraction testFraction = new FractionTools.Fraction(16, 12);
        FractionTools.Fraction[] atoms = FractionTools.AtomizeFraction(testFraction);
        Assert.IsNotNull(atoms, "AtomizeFraction did not return anything");
        Assert.IsTrue(testFraction.numerator == 16 && testFraction.denominator == 12, "AtomizeFraction mutated its input");
        Assert.IsTrue(atoms.Length == 2, "AtomizeFraction did not break into largest chunks");
        Assert.IsTrue(atoms[0].numerator == 1 && atoms[0].denominator == 1, "AtomizeFraction didn't return 1/1 as expected");
        Assert.IsTrue(atoms[1].numerator == 1 && atoms[1].denominator == 3, "AtomizeFraction didn't return 1/3 as expected");
    }

    [Test]
    public void AtomizeFractionNoBreakingTest()
    {
        FractionTools.Fraction testFraction = new FractionTools.Fraction(1, 12);
        FractionTools.Fraction[] atoms = FractionTools.AtomizeFraction(testFraction);
        Assert.IsNotNull(atoms, "AtomizeFraction did not return anything");
        Assert.IsTrue(testFraction.numerator == 1 && testFraction.denominator == 12, "AtomizeFraction mutated its input");
        Assert.IsTrue(atoms.Length == 1, "AtomizeFraction broke an already atomic fratcion");
        Assert.IsTrue(atoms[0].numerator == 1 && atoms[0].denominator == 12, "AtomizeFraction didn't return 1/12 as expected");
    }

    [Test]
    public void AtomizeFractionIgnoreOnesTest()
    {
        FractionTools.Fraction testFraction = new FractionTools.Fraction(16, 12);
        FractionTools.Fraction[] atoms = FractionTools.AtomizeFraction(testFraction, false);
        Assert.IsNotNull(atoms, "AtomizeFraction did not return anything");
        Assert.IsTrue(testFraction.numerator == 16 && testFraction.denominator == 12, "AtomizeFraction mutated its input");
        Assert.IsTrue(atoms.Length == 2, "AtomizeFraction did not break into largest chunks");
        Assert.IsTrue(atoms[0].numerator == 2 && atoms[0].denominator == 2, "AtomizeFraction didn't return 2/2 as expected");
        Assert.IsTrue(atoms[1].numerator == 1 && atoms[1].denominator == 3, "AtomizeFraction didn't return 1/3 as expected");
    }

    [Test]
    public void AtomizeFractionWithOnesTest()
    {
        FractionTools.Fraction testFraction = new FractionTools.Fraction(1, 1);
        FractionTools.Fraction[] atoms = FractionTools.AtomizeFraction(testFraction, true);
        Assert.IsNotNull(atoms, "AtomizeFraction did not return anything");
        Assert.IsTrue(testFraction.numerator == 1 && testFraction.denominator == 1, "AtomizeFraction mutated its input");
        Assert.IsTrue(atoms.Length == 1, "AtomizeFraction did not break into largest chunks, (returned " + atoms.Length + " items)");
        Assert.IsTrue(atoms[0].numerator == 1 && atoms[0].denominator == 1, "AtomizeFraction didn't return 1/1 as expected");
    }
}