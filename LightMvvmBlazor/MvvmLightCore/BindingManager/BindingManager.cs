﻿using System.Reflection;

namespace MvvmLightCore.Binder
{
    public class BindingManager : IBindingManager
    {
        /// <summary>
        /// Mapping of VM with object id and object VM with corresponding property.
        /// </summary>
        private readonly Dictionary<int, List<IBindableObject>> propVmMapping;
        public BindingManager()
        {
            propVmMapping = new Dictionary<int, List<IBindableObject>>();
        }

        public void AddBinding(IBindableObject toFindBindObject)
        {
            if (toFindBindObject.NotifyObj != null && toFindBindObject.Properties.First() != null)
            {
                if (!propVmMapping.ContainsKey(toFindBindObject.GetHashcode))
                {
                    var IBindableObject = new BindableObject(toFindBindObject.NotifyObj);
                    propVmMapping.Add(toFindBindObject.GetHashcode, new List<IBindableObject>());
                    IBindableObject.Properties.Add(toFindBindObject.Properties.First());
                    propVmMapping[toFindBindObject.GetHashcode].Add(IBindableObject);
                }
                else
                {
                    //If prop and vm already exist
                    if (CheckIfBindingAlreadyExist(toFindBindObject))
                    {
                        return;
                    }
                    else
                    {
                        //Update the prop info in the view model.
                        var vm = (from obj in  this.propVmMapping[toFindBindObject.GetHashcode] where obj.NotifyObjAlreadyExist(toFindBindObject) select obj).First();
                        vm.Properties.Add(toFindBindObject.Properties.First());
                    }

                }
            }
        }

        public bool CheckIfBindingAlreadyExist(IBindableObject toFindBindObject)
        {
            return this.propVmMapping.ContainsKey(toFindBindObject.GetHashcode) &&
                                  this.propVmMapping[toFindBindObject.GetHashcode].Any(obj => obj.NotifyObjPropAlreadyExist(toFindBindObject));
        }

        public void RemoveBinding(IBindableObject IBindableObject)
        {
            //todo:db Will do later.
            //if (IBindableObject.ViewModel != null && propVmMapping.ContainsKey(IBindableObject.ViewModel)
            //    && propVmMapping[IBindableObject.ViewModel].ContainsKey(IBindableObject.Property))
            //{
            //    propVmMapping[IBindableObject.ViewModel].Remove(IBindableObject.Property);
            //    if (propVmMapping[IBindableObject.ViewModel].Count() == 0)
            //    {
            //        propVmMapping.Remove(IBindableObject.ViewModel);
            //    }
            //}
        }

        public PropertyInfo? GetBindableProperty(IBindableObject IBindableObject)
        {
            if (CheckIfBindingAlreadyExist(IBindableObject))
            {
              var bindableObj =  (from ele in this.propVmMapping[IBindableObject.GetHashcode] where ele.NotifyObjAlreadyExist(IBindableObject) select ele).First();
                if (bindableObj.Properties.TryGetValue(bindableObj.Properties.First(), out PropertyInfo? propertyInfo))
                {
                    return propertyInfo;
                }
            }
            throw new KeyNotFoundException($"Bindable object does not exist " + " Property " + IBindableObject?.Properties?.First()?.Name);
        }
    }
}
