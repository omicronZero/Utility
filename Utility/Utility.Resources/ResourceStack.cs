using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Data.Resources
{
    public class ResourceStack
    {
        private readonly Stack<(IResource Resource, IDisposable Handle)> _resources;
        private readonly bool _releaseNonTop;

        public bool AllowInactivity { get; }

        public ResourceStack(bool releaseNonTop, bool allowInactivity)
        {
            _resources = new Stack<(IResource, IDisposable)>();
            _releaseNonTop = releaseNonTop;
            AllowInactivity = allowInactivity;
        }

        public int Count => _resources.Count;

        public bool IsActive => _resources.Count > 0 && _resources.Peek().Item1 != null;

        public void Clear()
        {
            if (_resources.Count == 0)
                return;

            var rh = _resources.Peek();
            IResource r = rh.Resource;
            bool active = r != null;

            if (active)
                Unfocused(r);

            if (!_releaseNonTop)
            {
                foreach(var p in _resources)
                {
                    //all resources of the previous frame have already been released
                    if (p.Resource == null)
                        break;

                    if (!_releaseNonTop)
                        p.Handle.Dispose();
                }
            }
            else
            {
                rh.Handle?.Dispose();
            }
            _resources.Clear();

            if (active)
                InactivityChanged(false);
        }

        public void ClearFrame()
        {
            if (_resources.Count == 0)
                return;

            IResource r = _resources.Peek().Resource;
            bool active = r != null;

            if (active)
                Unfocused(r);
            else
            {
                _resources.Pop();

                if (_resources.Count > 0 && _resources.Peek().Resource != null)
                    InactivityChanged(true);

                return;
            }

            while (_resources.Count > 0)
            {
                var p = _resources.Peek();

                if (p.Resource == null)
                    break;

                if (!_releaseNonTop)
                    p.Handle.Dispose();

                _resources.Pop();
            }

            if (active)
                InactivityChanged(false);
        }

        //merge: merges with previous item, if it is an inactivity element
        public bool PushInactive(bool merge)
        {
            if (merge && !IsActive)
                return false;

            Push(null);

            return true;
        }

        public void Push(IResource resource)
        {
            if (!AllowInactivity && resource == null)
                throw new ArgumentNullException(nameof(resource), "Resource must not be null as inactivity elements are not allowed.");

            bool active = IsActive; //stored to track, whether inactivity has changed by the current operation

            IDisposable handle = resource?.Allocate();

            if (_resources.Count > 0)
            {
                (IResource prev, IDisposable h) = _resources.Peek();
                Unfocused(prev);

                //if _releaseNonTop, the previous top item is to be released
                if (_releaseNonTop)
                {
                    h?.Dispose();
                }
            }

            //pushes the new resource and it's allocated handle
            _resources.Push((resource, handle));

            if (resource == null) //an inactivity item (null) was pushed
            {
                //either all previous resources have already been released per configuration or this has to be done now
                if (!_releaseNonTop)
                {
                    //deactivates all resources down to the last inactivity element (null)
                    foreach ((IResource p, IDisposable h) in _resources)
                    {
                        if (h == null)
                            break;

                        h.Dispose();
                    }
                }

                //activity state has changed to active
                if (active)
                    InactivityChanged(true);
            }
            else
            {
                //the indicated resource was activated
                Focused(resource);

                //activity state has changed to inactive
                if (!active)
                    InactivityChanged(false);
            }
        }

        public void Pop()
        {
            if (_resources.Count == 0)
                throw new InvalidOperationException("The resource stack is empty.");

            (IResource resource, IDisposable handle) = _resources.Pop();

            if (resource != null)
            {
                Unfocused(resource);
                handle.Dispose();
            }
            else if (_releaseNonTop)
            {
                Stack<IResource> rs = null;

                //get all resources previously released due to the inactivity element
                //(down to the previous inactivity element, if available)...
                while (_resources.Count > 0)
                {
                    (IResource p, IDisposable h) = _resources.Peek();

                    if (h == null)
                        break;

                    if (rs == null)
                        rs = new Stack<IResource>();

                    _resources.Pop();
                    rs.Push(p);
                }

                //...and reallocate them
                if (rs != null)
                {
                    while (rs.Count > 0)
                    {
                        IResource rsc = rs.Pop();
                        _resources.Push((rsc, rsc.Allocate()));
                    }
                }
            }

            if (_resources.Count > 0)
            {
                if (_releaseNonTop)
                {
                    (resource, handle) = _resources.Pop();

                    _resources.Push((resource, resource?.Allocate()));

                    if (resource != null)
                        Focused(resource);
                }
            }
        }

        protected virtual void InactivityChanged(bool inactive)
        { }

        protected virtual void Focused(IResource resource)
        { }

        protected virtual void Unfocused(IResource resource)
        { }
    }
}
