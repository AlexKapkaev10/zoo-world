using System;

namespace Project.UI.MVP
{
    public interface IInfoPresenter : IDisposable
    {
        void SetActive(bool isActive);
    }
}