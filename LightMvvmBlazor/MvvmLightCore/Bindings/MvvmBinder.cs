﻿using MvvmLightCore.Binder;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmLightCore
{
    public class MvvmBinder : IDisposable, IMvvmBinder
    {
        private readonly IBindingManager bindingManager;

        public event PropertyChangedEventHandler? ViewModelPropertyChanged;

        public MvvmBinder(IBindingManager bindingManager)
        {
            this.bindingManager = bindingManager;
        }

        public TValue? Bind<TInput, TValue>(INotifyPropertyChanged viewmodel, Expression<Func<TInput, TValue>> bindingExpression) where TInput : INotifyPropertyChanged
        {
            var bindableProperty = ParseBindingExpression(bindingExpression);
            IBindableObject bindableObj = new BindableObject(new WeakReference<INotifyPropertyChanged>(viewmodel));
            bindableObj.Properties.Add(bindableProperty);
            if (!this.bindingManager.CheckIfBindingAlreadyExist(bindableObj))
            {
                viewmodel.PropertyChanged -= this.ViewModelPropertyChanged;
                viewmodel.PropertyChanged += this.ViewModelPropertyChanged;
                this.bindingManager.AddBinding(bindableObj);
            }
            return (TValue)bindableProperty?.GetValue(viewmodel);
        }

        private PropertyInfo? ParseBindingExpression<TInput, TValue>(Expression<Func<TInput, TValue>> bindingExpression) where TInput : INotifyPropertyChanged
        {
            if (bindingExpression.NodeType == ExpressionType.Lambda && bindingExpression.Body is MemberExpression && (bindingExpression.Body as MemberExpression).Member is PropertyInfo)
            {
                return (bindingExpression?.Body as MemberExpression)?.Member as PropertyInfo;
            }
            throw new NotSupportedException();
        }

        public void Dispose()
        {

        }
    }
}
