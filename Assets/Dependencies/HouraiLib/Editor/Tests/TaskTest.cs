using System;
using System.Linq;
using NUnit.Framework;

namespace HouraiTeahouse {

    public class TaskNonGenericTest {

        [Test]
        public void can_resolve_simple_task() {
            ITask task = Task.Resolved;
            var completed = 0;
            task.Then(() => ++completed);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_simple_task() {
            var ex = new Exception();
            var task = Task.FromError(ex);
            var errors = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++errors;
            });
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void exception_is_thrown_for_reject_after_reject() {
            var task = new Task();
            task.Reject(new Exception());
            Assert.Throws<InvalidOperationException>(() =>
                task.Reject(new Exception())
            );
        }

        [Test]
        public void exception_is_thrown_for_reject_after_resolve() {
            var task = new Task();
            task.Resolve();
            Assert.Throws<InvalidOperationException>(() =>
                task.Reject(new Exception())
            );
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_reject() {
            var task = new Task();
            task.Reject(new Exception());
            Assert.Throws<InvalidOperationException>(() =>
                task.Resolve()
            );
        }

        [Test]
        public void can_resolve_task_and_trigger_then_handler() {
            var task = new Task();
            var completed = 0;
            task.Then(() => ++completed);
            task.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_resolve() {
            var task = new Task();
            task.Resolve();
            Assert.Throws<InvalidOperationException>(() =>
                task.Resolve()
            );
        }

        [Test]
        public void can_resolve_task_and_trigger_multiple_then_handlers_in_order() {
            var task = new Task();
            var completed = 0;
            task.Then(() => Assert.AreEqual(1, ++completed));
            task.Then(() => Assert.AreEqual(2, ++completed));
            task.Resolve();
            Assert.AreEqual(2, completed);
        }

        [Test]
        public void can_resolve_task_and_trigger_then_handler_with_callback_registration_after_resolve() {
            var task = new Task();
            var completed = 0;
            task.Resolve();
            task.Then(() => ++completed);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_task_and_trigger_error_handler() {
            var task = new Task();
            var ex = new ApplicationException();
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++completed;
            });
            task.Reject(ex);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_task_and_trigger_multiple_error_handlers_in_order() {
            var task = new Task();
            var ex = new ApplicationException();
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                Assert.AreEqual(1, ++completed);
            });
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                Assert.AreEqual(2, ++completed);
            });
            task.Reject(ex);
            Assert.AreEqual(2, completed);
        }

        [Test]
        public void can_reject_task_and_trigger_error_handler_with_registration_after_reject() {
            var task = new Task();
            var ex = new ApplicationException();
            task.Reject(ex);
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void error_handler_is_not_invoked_for_resolved_taskd() {
            var task = new Task();
            task.Catch(e => {
                throw new ApplicationException("This shouldn't happen");
            });
            task.Resolve();
        }

        [Test]
        public void then_handler_is_not_invoked_for_rejected_task() {
            var task = new Task();
            task.Then(() => {
                throw new ApplicationException("This shouldn't happen");
            });

            task.Reject(new ApplicationException("Rejection!"));
        }

        [Test]
        public void chain_multiple_tasks_using_all() {
            var task = new Task();
            var chainedPromise1 = new Task();
            var chainedPromise2 = new Task();
            var completed = 0;
            task.ThenAll(chainedPromise1, chainedPromise2)
                .Then(() => ++completed);
            Assert.AreEqual(0, completed);
            task.Resolve();
            Assert.AreEqual(0, completed);

            chainedPromise1.Resolve();

            Assert.AreEqual(0, completed);

            chainedPromise2.Resolve();

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void chain_multiple_tasks_using_all_that_are_resolved_out_of_order() {
            var task = new Task();
            var chainedPromise1 = new Task<int>();
            var chainedPromise2 = new Task<int>();
            var chainedResult1 = 10;
            var chainedResult2 = 15;
            var completed = 0;
            task.ThenAll(chainedPromise1, chainedPromise2)
                .Then(result => {
                    var items = result.ToArray();
                    Assert.AreEqual(2, items.Length);
                    Assert.AreEqual(chainedResult1, items[0]);
                    Assert.AreEqual(chainedResult2, items[1]);

                    completed++;
                });
            Assert.AreEqual(0, completed);
            task.Resolve();
            Assert.AreEqual(0, completed);
            chainedPromise1.Resolve(chainedResult1);
            Assert.AreEqual(0, completed);
            chainedPromise2.Resolve(chainedResult2);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void chain_multiple_value_tasks_using_all_resolved_out_of_order() {
            var task = new Task();
            var chainedPromise1 = new Task<int>();
            var chainedPromise2 = new Task<int>();
            var chainedResult1 = 10;
            var chainedResult2 = 15;
            var completed = 0;
            task.ThenAll(chainedPromise1, chainedPromise2)
                .Then(result => {
                    var items = result.ToArray();
                    Assert.AreEqual(2, items.Length);
                    Assert.AreEqual(chainedResult1, items[0]);
                    Assert.AreEqual(chainedResult2, items[1]);

                    completed++;
                });

            Assert.AreEqual(0, completed);
            task.Resolve();
            Assert.AreEqual(0, completed);
            chainedPromise2.Resolve(chainedResult2);
            Assert.AreEqual(0, completed);
            chainedPromise1.Resolve(chainedResult1);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_resolved_when_children_are_resolved() {
            var task1 = new Task();
            var task2 = new Task();
            var all = Task.All(task1, task2);
            var completed = 0;
            all.Then(() => completed++);
            task1.Resolve();
            task2.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_rejected_when_first_task_is_rejected() {
            var task1 = new Task();
            var task2 = new Task();
            var all = Task.All(task1, task2);
            all.Then(() => {
                throw new ApplicationException("Shouldn't happen");
            });
            var errors = 0;
            all.Catch(e => {
                ++errors;
            });
            task1.Reject(new ApplicationException("Error!"));
            task2.Resolve();
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_rejected_when_second_task_is_rejected() {
            var task1 = new Task();
            var task2 = new Task();
            var all = Task.All(task1, task2);
            all.Then(() => {
                throw new ApplicationException("Shouldn't happen");
            });
            var errors = 0;
            all.Catch(e => {
                errors++;
            });
            task1.Resolve();
            task2.Reject(new ApplicationException("Error!"));
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_rejected_when_both_tasks_are_rejected() {
            var task1 = new Task();
            var task2 = new Task();
            var all = Task.All(task1, task2);
            all.Then(() => {
                throw new ApplicationException("Shouldn't happen");
            });
            var errors = 0;
            all.Catch(e => {
                errors++;
            });
            task1.Reject(new ApplicationException("Error!"));
            task2.Reject(new ApplicationException("Error!"));
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_resolved_if_there_are_no_tasks() {
            var all = Task.All(Enumerable.Empty<ITask>());
            var completed = 0;
            all.Then(() => completed++);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_resolved_when_all_tasks_are_already_resolved() {
            var task1 = Task.Resolved;
            var task2 = Task.Resolved;
            var all = Task.All(task1, task2);
            var completed = 0;
            all.Then(() => {
                completed++;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_during_then_rejects_task() {
            var task = new Task();
            var errors = 0;
            var ex = new Exception();
            task.Then(() => {
                    throw ex;
                }).Catch(e => {
                    Assert.AreEqual(ex, e);
                    errors++;
                });
            task.Resolve();
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void can_chain_task() {
            var task = new Task();
            var chainedPromise = new Task();
            var completed = 0;
            task.Then(() => chainedPromise)
                .Then(() => ++completed);
            task.Resolve();
            chainedPromise.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_chain_task_and_convert_to_task_that_yields_a_value() {
            var task = new Task();
            var chainedPromise = new Task<string>();
            var chainedPromiseValue = "some-value";
            var completed = 0;
            task.Then(() => chainedPromise)
                .Then(v => {
                    Assert.AreEqual(chainedPromiseValue, v);
                    ++completed;
                });
            task.Resolve();
            chainedPromise.Resolve(chainedPromiseValue);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_in_chain_rejects_resulting_task() {
            var task = new Task();
            var ex = new Exception();
            var errors = 0;
            task.Then(() => {
                    throw ex;
                })
                .Catch(e => {
                    Assert.AreEqual(ex, e);
                    ++errors;
                });
            task.Resolve();
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void rejection_of_source_task_rejects_chained_task() {
            var task = new Task();
            var chainedPromise = new Task();
            var ex = new Exception();
            var errors = 0;
            task.Then(() => chainedPromise)
                .Catch(e => {
                    Assert.AreEqual(ex, e);
                    ++errors;
                });
            task.Reject(ex);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void race_is_resolved_when_first_task_is_resolved_first() {
            var task1 = new Task();
            var task2 = new Task();
            var completed = 0;
            Task.Any(task1, task2)
                .Then(() => completed++);
            task1.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void race_is_resolved_when_second_task_is_resolved_first() {
            var task1 = new Task();
            var task2 = new Task();
            var completed = 0;
            Task.Any(task1, task2)
                .Then(() => completed++);
            task2.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void race_is_rejected_when_first_task_is_rejected_first() {
            var task1 = new Task();
            var task2 = new Task();
            Exception ex = null;
            Task.Any(task1, task2)
                .Catch(e => ex = e);
            var expected = new Exception();
            task1.Reject(expected);
            Assert.AreEqual(expected, ex);
        }

        [Test]
        public void race_is_rejected_when_second_task_is_rejected_first() {
            var task1 = new Task();
            var task2 = new Task();
            Exception ex = null;
            Task.Any(task1, task2)
                .Catch(e => ex = e);
            var expected = new Exception();
            task2.Reject(expected);
            Assert.AreEqual(expected, ex);
        }

        [Test]
        public void sequence_with_no_operations_is_directly_resolved() {
            var completed = 0;
            Task.Sequence()
                .Then(() => completed++);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequenced_is_not_resolved_when_operation_is_not_resolved() {
            var completed = 0;
            Task.Sequence(() => new Task())
                .Then(() => ++completed);
            Assert.AreEqual(0, completed);
        }

        [Test]
        public void sequence_is_resolved_when_operation_is_resolved() {
            var completed = 0;
            Task.Sequence(() => Task.Resolved)
                .Then(() => ++completed);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequence_is_unresolved_when_some_operations_are_unresolved() {
            var completed = 0;
            Task.Sequence(
                    () => Task.Resolved,
                    () => new Task()
                ).Then(() => ++completed);
            Assert.AreEqual(0, completed);
        }

        [Test]
        public void sequence_is_resolved_when_all_operations_are_resolved() {
            var completed = 0;
            Task.Sequence(
                    () => Task.Resolved,
                    () => Task.Resolved
                ).Then(() => ++completed);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void sequenced_operations_are_run_in_order_is_directly_resolved() {
            var order = 0;
            Task.Sequence(
                    () => {
                        Assert.AreEqual(1, ++order);
                        return Task.Resolved;
                    },
                    () => {
                        Assert.AreEqual(2, ++order);
                        return Task.Resolved;
                    },
                    () => {
                        Assert.AreEqual(3, ++order);
                        return Task.Resolved;
                    }
                );
            Assert.AreEqual(3, order);
        }

        [Test]
        public void exception_thrown_in_sequence_rejects_the_task() {
            var errored = 0;
            var completed = 0;
            var ex = new Exception();
            Task .Sequence(() => {
                    throw ex;
                }) .Catch(e => {
                    Assert.AreEqual(ex, e);
                    ++errored;
                }) .Then(() => ++completed);
            Assert.AreEqual(1, errored);
            Assert.AreEqual(0, completed);
        }

        [Test]
        public void exception_thrown_in_sequence_stops_following_operations_from_being_invoked() {
            var completed = 0;
            Task.Sequence(
                    () => {
                        ++completed;
                        return Task.Resolved;
                    },
                    () => {
                        throw new Exception();
                    },
                    () => {
                        ++completed;
                        return Task.Resolved;
                    }
                );
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_resolve_task_via_resolver_function() {
            var task = new Task((resolve, reject) => {
                resolve();
            });
            var completed = 0;
            task.Then(() => {
                completed++;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_task_via_reject_function() {
            var ex = new Exception();
            var task = new Task((resolve, reject) => {
                reject(ex);
            });
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_during_resolver_rejects_proimse() {
            var ex = new Exception();
            var task = new Task((resolve, reject) => {
                throw ex;
            });
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                completed++;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void unhandled_exception_is_propagated_via_event() {
            var task = new Task();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => {
                Assert.AreEqual(ex, e.ExceptionObject);
                ++eventRaised;
            };
            Task.UnhandledException += handler;
            try {
                task.Then(() => {
                        throw ex;
                    }).Done();
                task.Resolve();
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void exception_in_done_callback_is_propagated_via_event() {
            var task = new Task();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => {
                Assert.AreEqual(ex, e.ExceptionObject);
                ++eventRaised;
            };
            Task.UnhandledException += handler;
            try {
                task.Then(() => { throw ex; }).Done();
                task.Resolve();
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void handled_exception_is_not_propagated_via_event() {
            var task = new Task();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => eventRaised++;
            Task.UnhandledException += handler;
            try {
                task.Then(() => {
                        throw ex;
                    }).Catch(_ => {
                        // Catch the error.
                    }).Done();
                task.Resolve();
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void can_handle_Done_onResolved() {
            var task = new Task();
            var callback = 0;
            task.Then(() => ++callback).Done();
            task.Resolve();
            Assert.AreEqual(1, callback);
        }

        [Test]
        public void can_handle_Done_onResolved_with_onReject() {
            var task = new Task();
            var callback = 0;
            var errorCallback = 0;
            task.Then(() => callback++).Catch(ex => errorCallback++).Done();
            task.Resolve();
            Assert.AreEqual(1, callback);
            Assert.AreEqual(0, errorCallback);
        }

        /*todo:
         * Also want a test that exception thrown during Then triggers the error handler.
         * How do Javascript tasks work in this regard?
        [Test]
        public void exception_during_Done_onResolved_triggers_error_hander()
        {
            var task = new Task();
            var callback = 0;
            var errorCallback = 0;
            var expectedValue = 5;
            var expectedException = new Exception();
            task.Done(
                value =>
                {
                    Assert.AreEqual(expectedValue, value);
                    ++callback;
                    throw expectedException;
                },
                ex =>
                {
                    Assert.AreEqual(expectedException, ex);
                    ++errorCallback;
                }
            );
            task.Resolve(expectedValue);
            Assert.AreEqual(1, callback);
            Assert.AreEqual(1, errorCallback);
        }
         * */

        [Test]
        public void exception_during_Then_onResolved_triggers_error_hander() {
            var task = new Task();
            var callback = 0;
            var errorCallback = 0;
            var expectedException = new Exception();
            task.Then(() => { throw expectedException; }).Then(() => ++callback).Catch(ex => {
                Assert.AreEqual(expectedException, ex);
                ++errorCallback;
            }).Done();
            task.Resolve();
            Assert.AreEqual(0, callback);
            Assert.AreEqual(1, errorCallback);
        }

        [Test]
        public void inner_exception_handled_by_outer_task() {
            var task = new Task();
            var errorCallback = 0;
            var expectedException = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => ++eventRaised;
            Task.UnhandledException += handler;
            try {
                task.Then(() => {
                        return Task.Resolved.Then(() => {
                            throw expectedException;
                        });
                    }) .Catch(ex => {
                        Assert.AreEqual(expectedException, ex);
                        ++errorCallback;
                    });
                task.Resolve();
                // No "done" in the chain, no generic event handler should be called
                Assert.AreEqual(0, eventRaised);
                // Instead the catch should have got the exception
                Assert.AreEqual(1, errorCallback);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void inner_exception_handled_by_outer_task_with_results() {
            var task = new Task<int>();
            var errorCallback = 0;
            var expectedException = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => ++eventRaised;
            Task.UnhandledException += handler;
            try {
                task.Then(_ => {
                        return Task.FromResult(5).Then(__ => {
                            throw expectedException;
                        });
                    }).Catch(ex => {
                        Assert.AreEqual(expectedException, ex);
                        errorCallback++;
                    });
                task.Resolve(2);
                // No "done" in the chain, no generic event handler should be called
                Assert.AreEqual(0, eventRaised);
                // Instead the catch should have got the exception
                Assert.AreEqual(1, errorCallback);
            } finally {
                Task.UnhandledException -= handler;
            }
        }
    }
}
