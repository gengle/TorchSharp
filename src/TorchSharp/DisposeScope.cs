﻿// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using TorchSharp.Utils;

#nullable enable
namespace TorchSharp
{
    /// <summary>
    /// Keeps track of all disposables that are in the current scope - the dispose scopes can be nested and the
    /// nesting functionality is mainly managed by DisposeScopeManager.
    /// </summary>
    public class DisposeScope : IDisposable
    {
        private readonly DisposeScopeManager _disposeScopeManager;

        public DisposeScope(DisposeScopeManager disposeScopeManager)
        {
            _disposeScopeManager = disposeScopeManager ?? throw new ArgumentNullException(nameof(disposeScopeManager));
            if (disposeScopeManager.DisposeScopeStack.Count > 0) {
                OuterScope = disposeScopeManager.DisposeScopeStack.Peek();
            }
        }

        /// <summary>
        /// The outer scope with relation to this scope.
        /// </summary>
        internal DisposeScope? OuterScope { get; }

        /// <summary>
        /// The disposables that are scheduled for disposing.
        /// </summary>
        /// TODO: There is a ReferenceEqualityComparer coming in .NET 6, use that!
        internal HashSet<IDisposable> Disposables { get; private set; } =
            new HashSet<IDisposable>(ReferenceEqualityComparer<IDisposable>.Default);

        /// <summary>
        /// A view of the disposables in the scope - this list will not be kept in synch with the disposables
        /// in the scope.
        /// </summary>
        public IReadOnlyList<IDisposable> DisposablesView => Disposables.ToList();

        /// <summary>
        /// The number of disposables currently held in the scope
        /// </summary>
        public int DisposablesCount => Disposables.Count;

        /// <summary>
        /// Includes a disposable in the scope - for tensors this is done automatically once the scope has been
        /// created. Use this method to add additional disposables that should be disposed, but you typically
        /// don't need to call this method.
        /// </summary>
        /// <param name="disposable">The disposable to keep in the scope</param>
        /// <returns></returns>
        public T Include<T>(T disposable) where T : IDisposable
        {
            Disposables.Add(disposable);
            return disposable;
        }

        /// <summary>
        /// Excludes a set of tensors/disposables from the current dispose scope, and moves it up to the outer
        /// dispose scope, if one exists. See overloaded methods. If you wish to exclude a tensor from all sccopes,
        /// use Detach.
        /// </summary>
        public T MoveToOuter<T>(T disposable) where T : IDisposable
        {
            MoveToOuter(new IDisposable[] { disposable });
            return disposable;
        }

        /// <summary>
        /// Excludes a set of tensors/disposables from the current dispose scope, and moves it up to the outer
        /// dispose scope, if one exists. See overloaded methods. If you wish to exclude a tensor from all sccopes,
        /// use Detach.
        /// </summary>
        public (T1 first, T2 second) MoveToOuter<T1, T2>(T1 first, T2 second)
            where T1 : IDisposable where T2 : IDisposable
        {
            MoveToOuter(new IDisposable[] { first, second });
            return (first, second);
        }

        /// <summary>
        /// Excludes a set of tensors/disposables from the current dispose scope, and moves it up to the outer
        /// dispose scope, if one exists. See overloaded methods. If you wish to exclude a tensor from all sccopes,
        /// use Detach.
        /// </summary>
        public (T1 first, T2 second, T3 third) MoveToOuter<T1, T2, T3>(T1 first, T2 second, T3 third)
            where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable
        {
            MoveToOuter(new IDisposable[] { first, second, third });
            return (first, second, third);
        }

        /// <summary>
        /// Excludes a set of tensors/disposables from the current dispose scope, and moves it up to the outer
        /// dispose scope, if one exists. See overloaded methods. If you wish to exclude a tensor from all sccopes,
        /// use Detach.
        /// </summary>
        public void MoveToOuter(params IDisposable[] disposables) =>
            MoveToOuter((IEnumerable<IDisposable>)disposables);

        /// <summary>
        /// Excludes a set of tensors/disposables from the current dispose scope, and moves it up to the outer
        /// dispose scope, if one exists. See overloaded methods. If you wish to exclude a tensor from all sccopes,
        /// use Detach.
        /// </summary>
        public void MoveToOuter(IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables) {
                if (Disposables.Remove(disposable)) {
                    AddToParent(disposable);
                }
            }
        }

        /// <summary>
        /// Detaches/excludes a set of tensors/disposables from the all dispose scopes, see overloaded methods. See MoveToOuter
        /// if you wish to move it to the outer dispose scope.
        /// </summary>
        public T Detach<T>(T disposable) where T : IDisposable
        {
            Detach(new IDisposable[] { disposable });
            return disposable;
        }

        /// <summary>
        /// Detaches/excludes a set of tensors/disposables from the all dispose scopes, see overloaded methods. See MoveToOuter
        /// if you wish to move it to the outer dispose scope.
        /// </summary>
        public (T1 first, T2 second) Detach<T1, T2>(T1 first, T2 second)
            where T1 : IDisposable where T2 : IDisposable
        {
            Detach(new IDisposable[] { first, second });
            return (first, second);
        }

        /// <summary>
        /// Detaches/excludes a set of tensors/disposables from the all dispose scopes, see overloaded methods. See MoveToOuter
        /// if you wish to move it to the outer dispose scope.
        /// </summary>
        public (T1 first, T2 second, T3 third) Detach<T1, T2, T3>(T1 first, T2 second, T3 third)
            where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable
        {
            Detach(new IDisposable[] { first, second, third });
            return (first, second, third);
        }

        /// <summary>
        /// Detaches/excludes a set of tensors/disposables from the all dispose scopes, see overloaded methods. See MoveToOuter
        /// if you wish to move it to the outer dispose scope.
        /// </summary>
        public void Detach(params IDisposable[] disposables) => Detach((IEnumerable<IDisposable>)disposables);

        /// <summary>
        /// Detaches/excludes a set of tensors/disposables from the all dispose scopes, see overloaded methods. See MoveToOuter
        /// if you wish to move it to the outer dispose scope.
        /// </summary>
        public void Detach(IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables) {
                if (Disposables.Remove(disposable)) {
                    _disposeScopeManager.StatisticsInstance.DetachedFromScopeCount++;
                    if (disposable is torch.Tensor tensor) {
                        tensor.OwningDisposeScope = null;
                    }
                }
            }
        }

        /// <summary>
        /// Disposes everything currently in the dispose scope.
        /// </summary>
        public void DisposeEverything() => DisposeEverythingBut(Enumerable.Empty<IDisposable>());

        /// <summary>
        /// As an intermediate step, you can dispose all the tensors/disposables currently scheduled for dispose, to
        /// clear up some memory without creating a new scope. Note that this doesn't permanently exclude the
        /// tensors from disposing, use Exclude for that. Also, excluded tensors don't need to be included
        /// here.
        /// </summary>
        public void DisposeEverythingBut(IEnumerable<IDisposable> inKeep)
        {
            // Avoiding multiple enumerations
            var oldList = Disposables;
            Disposables = inKeep.ToHashSet(ReferenceEqualityComparer<IDisposable>.Default);
            foreach (var disposable in oldList) {
                if (Disposables.Contains(disposable)) {
                    continue;
                }

                if (disposable is torch.Tensor tensor) {
                    // No need to have the disposable call back to the scope
                    tensor.OwningDisposeScope = null;
                    if (!tensor.IsInvalid) {
                        _disposeScopeManager.StatisticsInstance.DisposedInScopeCount++;
                    }
                } else {
                    _disposeScopeManager.StatisticsInstance.DisposedInScopeCount++;
                }

                disposable.Dispose();
            }
        }

        /// <summary>
        /// As an intermediate step, you can dispose all the tensors/disposables currently scheduled for dispose, to
        /// clear up some memory without creating a new scope. Note that this doesn't permanently exclude the
        /// tensors from disposing, use Exclude for that. Also, excluded tensors don't need to be included
        /// here.
        /// </summary>
        public void DisposeEverythingBut(params IDisposable[] keep) =>
            DisposeEverythingBut((IEnumerable<IDisposable>)keep);

        /// <summary>
        /// As an intermediate step, you can dispose all the tensors/disposables currently scheduled for dispose, to
        /// clear up some memory without creating a new scope. Note that this doesn't permanently exclude the
        /// tensors from disposing, use Exclude for that. Also, excluded tensors don't need to be included
        /// here.
        /// </summary>
        public T DisposeEverythingBut<T>(T keep) where T : IDisposable
        {
            DisposeEverythingBut(new IDisposable[] { keep });
            return keep;
        }

        /// <summary>
        /// As an intermediate step, you can dispose all the tensors/disposables currently scheduled for dispose, to
        /// clear up some memory without creating a new scope. Note that this doesn't permanently exclude the
        /// tensors from disposing, use Exclude for that. Also, excluded tensors don't need to be included
        /// here.
        /// </summary>
        public (T1 first, T2 second) DisposeEverythingBut<T1, T2>(T1 first, T2 second)
            where T1 : IDisposable where T2 : IDisposable
        {
            DisposeEverythingBut(new IDisposable[] { first, second });
            return (first, second);
        }

        /// <summary>
        /// As an intermediate step, you can dispose all the tensors/disposables currently scheduled for dispose, to
        /// clear up some memory without creating a new scope. Note that this doesn't permanently exclude the
        /// tensors from disposing, use Exclude for that. Also, excluded tensors don't need to be included
        /// here.
        /// </summary>
        public (T1 first, T2 second, T3 third) DisposeEverythingBut<T1, T2, T3>(T1 first, T2 second, T3 third)
            where T1 : IDisposable where T2 : IDisposable where T3 : IDisposable
        {
            DisposeEverythingBut(new IDisposable[] { first, second, third });
            return (first, second, third);
        }

        /// <summary>
        /// Disposes of the DisposeScope and all the disposables in its list. You would typically not call this method,
        /// instead you should use a usings clause around the scope.
        /// </summary>
        public void Dispose()
        {
            DisposeEverything();
            _disposeScopeManager.RemoveDisposeScope(this);
        }

        /// <summary>
        /// A method that notifies the DisposeScope that a disposable was disposed, so that it can be removed from the
        /// tracked list. This will be called if a tensor is manually disposed, but you can also add your own
        /// disposables to the dispose scope. If you do, and dispose them manually, you should make sure to call this
        /// method.
        /// </summary>
        /// <param name="disposable">The disposable that was disposed</param>
        public void MarkAsDisposed(IDisposable disposable)
        {
            _disposeScopeManager.StatisticsInstance.DisposedInScopeCount++;
            Disposables.Remove(disposable);
            if (disposable is torch.Tensor tensor) {
                tensor.OwningDisposeScope = null;
            }
        }

        /// <summary>
        /// Checks if the DisposeScope contains the disposable
        /// </summary>
        /// <param name="disposable">The disposable that's searched for</param>
        /// <returns></returns>
        public bool Contains(IDisposable disposable) => Disposables.Contains(disposable);

        private void AddToParent(IDisposable disposable)
        {
            if (OuterScope != null) {
                OuterScope.Disposables.Add(disposable);
            } else {
                _disposeScopeManager.StatisticsInstance.DetachedFromScopeCount++;
            }

            if (disposable is torch.Tensor tensor) {
                tensor.OwningDisposeScope = OuterScope;
            }
        }
    }
}