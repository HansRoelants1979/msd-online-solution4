namespace Tc.Crm.Service.CacheBuckets
{
    public interface IReferenceBucket<out T>
    {
        T GetBy(string code);

        void FillBucket();
    }
}
