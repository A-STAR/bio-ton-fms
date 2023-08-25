namespace BioTonFMS.Version
{
    public delegate string TokenFunction();

    public delegate string TokenFunction<in T>(T arg);

    public delegate string TokenFunction<in T1, in T2>(T1 arg1, T2 arg2);
}