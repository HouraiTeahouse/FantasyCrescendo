using System;
using System.Linq;
using NUnit.Framework;

namespace HouraiTeahouse {

    [Parallelizable]
    public class TaskTest {

        [Test]
        public void can_resolve_simple_task() {
            var taskdValue = 5;
            ITask<int> task = Task.FromResult(taskdValue);
            var completed = 0;
            task.Then(v => {
                Assert.AreEqual(taskdValue, v);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_simple_task() {
            var ex = new Exception();
            ITask task = Task.FromError(ex);
            var errors = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++errors;
            });
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void exception_is_thrown_for_reject_after_reject() {
            var task = new Task<int>();
            task.Reject(new Exception());
            Assert.Throws<InvalidOperationException>(() => task.Reject(new Exception()));
        }

        [Test]
        public void exception_is_thrown_for_reject_after_resolve() {
            var task = new Task<int>();
            task.Resolve(5);
            Assert.Throws<InvalidOperationException>(() => task.Reject(new Exception()));
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_reject() {
            var task = new Task<int>();
            task.Reject(new Exception());
            Assert.Throws<InvalidOperationException>(() => task.Resolve(5));
        }

        [Test]
        public void can_resolve_task_and_trigger_then_handler() {
            var task = new Task<int>();
            var completed = 0;
            var taskdValue = 15;
            task.Then(v => {
                Assert.AreEqual(taskdValue, v);
                ++completed;
            });
            task.Resolve(taskdValue);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_is_thrown_for_resolve_after_resolve() {
            var task = new Task<int>();
            task.Resolve(5);
            Assert.Throws<InvalidOperationException>(() => task.Resolve(5));
        }

        [Test]
        public void can_resolve_task_and_trigger_multiple_then_handlers_in_order() {
            var task = new Task<int>();
            var completed = 0;
            task.Then(v => Assert.AreEqual(1, completed++));
            task.Then(v => Assert.AreEqual(2, completed++));
            task.Resolve(1);
            Assert.AreEqual(2, completed);
        }

        [Test]
        public void can_resolve_task_and_trigger_then_handler_with_callback_registration_after_resolve() {
            var task = new Task<int>();
            var completed = 0;
            int taskdValue = -10;
            task.Resolve(taskdValue);
            task.Then(v => {
                Assert.AreEqual(taskdValue, v);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_task_and_trigger_error_handler() {
            var task = new Task<int>();
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
            var task = new Task<int>();
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
            var task = new Task<int>();
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
            var task = new Task<int>();
            task.Catch(e => { throw new ApplicationException("This shouldn't happen"); });
            task.Resolve(5);
        }

        [Test]
        public void then_handler_is_not_invoked_for_rejected_task() {
            var task = new Task<int>();
            task.Then(v => { throw new ApplicationException("This shouldn't happen"); });
            task.Reject(new ApplicationException("Rejection!"));
        }

        [Test]
        public void chain_multiple_tasks_using_all() {
            var task = new Task<string>();
            var chainedTask1 = new Task<int>();
            var chainedTask2 = new Task<int>();
            var chainedResult1 = 10;
            var chainedResult2 = 15;
            var completed = 0;
            task.ThenAll(chainedTask1, chainedTask2).Then(result => {
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(chainedResult1, result[0]);
                Assert.AreEqual(chainedResult2, result[1]);
                completed++;
            });
            Assert.AreEqual(0, completed);
            task.Resolve("hello");
            Assert.AreEqual(0, completed);
            chainedTask1.Resolve(chainedResult1);
            Assert.AreEqual(0, completed);
            chainedTask2.Resolve(chainedResult2);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void chain_multiple_tasks_using_all_that_are_resolved_out_of_order() {
            var task = new Task<string>();
            var chainedTask1 = new Task<int>();
            var chainedTask2 = new Task<int>();
            var chainedResult1 = 10;
            var chainedResult2 = 15;
            var completed = 0;
            task.ThenAll(chainedTask1, chainedTask2).Then(result => {
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(chainedResult1, result[0]);
                Assert.AreEqual(chainedResult2, result[1]);
                completed++;
            });
            Assert.AreEqual(0, completed);
            task.Resolve("hello");
            Assert.AreEqual(0, completed);
            chainedTask2.Resolve(chainedResult2);
            Assert.AreEqual(0, completed);
            chainedTask1.Resolve(chainedResult1);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void chain_multiple_tasks_using_all_and_convert_to_non_value_task() {
            var task = new Task<string>();
            var chainedTask1 = new Task();
            var chainedTask2 = new Task();
            var completed = 0;
            task.ThenAll(chainedTask1, chainedTask2).Then(() => { ++completed; });
            Assert.AreEqual(0, completed);
            task.Resolve("hello");
            Assert.AreEqual(0, completed);
            chainedTask1.Resolve();
            Assert.AreEqual(0, completed);
            chainedTask2.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_resolved_when_children_are_resolved() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();

            ITask<int[]> all = Task.All(task1, task2);

            var completed = 0;

            all.Then(v => {
                completed++;
                Assert.AreEqual(2, v.Length);
                Assert.AreEqual(1, v[0]);
                Assert.AreEqual(2, v[1]);
            });

            task1.Resolve(1);
            task2.Resolve(2);

            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_rejected_when_first_task_is_rejected() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            ITask<int[]> all = Task.All(task1, task2);
            all.Then(v => { throw new ApplicationException("Shouldn't happen"); });
            var errors = 0;
            all.Catch(e => { errors++; });
            task1.Reject(new ApplicationException("Error!"));
            task2.Resolve(2);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_rejected_when_second_task_is_rejected() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            ITask<int[]> all = Task.All(task1, task2);
            all.Then(v => { throw new ApplicationException("Shouldn't happen"); });
            var errors = 0;
            all.Catch(e => { ++errors; });
            task1.Resolve(2);
            task2.Reject(new ApplicationException("Error!"));
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_rejected_when_both_tasks_are_rejected() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            ITask<int[]> all = Task.All(task1, task2);
            all.Then(v => { throw new ApplicationException("Shouldn't happen"); });
            var errors = 0;
            all.Catch(e => { ++errors; });
            task1.Reject(new ApplicationException("Error!"));
            task2.Reject(new ApplicationException("Error!"));
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void combined_task_is_resolved_if_there_are_no_tasks() {
            ITask<int[]> all = Task.All(Enumerable.Empty<ITask<int>>());
            var completed = 0;
            all.Then(v => {
                completed++;
                Assert.IsEmpty(v);
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void combined_task_is_resolved_when_all_tasks_are_already_resolved() {
            ITask<int> task1 = Task.FromResult(1);
            ITask<int> task2 = Task.FromResult(1);
            ITask<ITask<int>[]> all = Task.All(Task.FromResults(task1, task2));
            var completed = 0;
            all.Then(v => {
                completed++;
                Assert.IsEmpty(v);
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_transform_task_value() {
            var task = new Task<int>();
            var taskdValue = 15;
            var completed = 0;
            task.Then(v => v.ToString()).Then(v => {
                Assert.AreEqual(taskdValue.ToString(), v);
                completed++;
            });
            task.Resolve(taskdValue);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void rejection_of_source_task_rejects_transformed_task() {
            var task = new Task<int>();
            var ex = new Exception();
            var errors = 0;
            task.Then(v => v.ToString()).Catch(e => {
                Assert.AreEqual(ex, e);
                errors++;
            });
            task.Reject(ex);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void exception_thrown_during_transform_rejects_transformed_task() {
            var task = new Task<int>();
            var taskdValue = 15;
            var errors = 0;
            var ex = new Exception();
            task.Then(v => { throw ex; }).Catch(e => {
                Assert.AreEqual(ex, e);
                errors++;
            });
            task.Resolve(taskdValue);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void can_chain_task_and_convert_type_of_value() {
            var task = new Task<int>();
            var chainedTask = new Task<string>();
            var taskdValue = 15;
            var chainedTaskValue = "blah";
            var completed = 0;
            task.Then<string>(v => chainedTask).Then(v => {
                Assert.AreEqual(chainedTaskValue, v);
                completed++;
            });
            task.Resolve(taskdValue);
            chainedTask.Resolve(chainedTaskValue);
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_chain_task_and_convert_to_non_value_task() {
            var task = new Task<int>();
            var chainedTask = new Task();
            var taskdValue = 15;
            var completed = 0;
            task.Then(v => (ITask) chainedTask).Then(() => { ++completed; });
            task.Resolve(taskdValue);
            chainedTask.Resolve();
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_in_chain_rejects_resulting_task() {
            var task = new Task<int>();
            var ex = new Exception();
            var errors = 0;
            task.Then(v => { throw ex; }).Catch(e => {
                Assert.AreEqual(ex, e);
                errors++;
            });
            task.Resolve(15);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void rejection_of_source_task_rejects_chained_task() {
            var task = new Task<int>();
            var chainedTask = new Task<string>();
            var ex = new Exception();
            var errors = 0;
            task.Then<string>(v => chainedTask).Catch(e => {
                Assert.AreEqual(ex, e);
                errors++;
            });
            task.Reject(ex);
            Assert.AreEqual(1, errors);
        }

        [Test]
        public void race_is_resolved_when_first_task_is_resolved_first() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            var resolved = 0;
            Task.Any(task1, task2).Then(i => resolved = i);
            task1.Resolve(5);
            Assert.AreEqual(5, resolved);
        }

        [Test]
        public void race_is_resolved_when_second_task_is_resolved_first() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            var resolved = 0;
            Task.Any(task1, task2).Then(i => resolved = i);
            task2.Resolve(12);
            Assert.AreEqual(12, resolved);
        }

        [Test]
        public void race_is_rejected_when_first_task_is_rejected_first() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            Exception ex = null;
            Task.Any(task1, task2).Catch(e => ex = e);
            var expected = new Exception();
            task1.Reject(expected);
            Assert.AreEqual(expected, ex);
        }

        [Test]
        public void race_is_rejected_when_second_task_is_rejected_first() {
            var task1 = new Task<int>();
            var task2 = new Task<int>();
            Exception ex = null;
            Task.Any(task1, task2).Catch(e => ex = e);
            var expected = new Exception();
            task2.Reject(expected);
            Assert.AreEqual(expected, ex);
        }

        [Test]
        public void can_resolve_task_via_resolver_function() {
            var task = new Task<int>((resolve, reject) => { resolve(5); });
            var completed = 0;
            task.Then(v => {
                Assert.AreEqual(5, v);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void can_reject_task_via_reject_function() {
            var ex = new Exception();
            var task = new Task<int>((resolve, reject) => { reject(ex); });
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void exception_thrown_during_resolver_rejects_task() {
            var ex = new Exception();
            var task = new Task<int>((resolve, reject) => { throw ex; });
            var completed = 0;
            task.Catch(e => {
                Assert.AreEqual(ex, e);
                ++completed;
            });
            Assert.AreEqual(1, completed);
        }

        [Test]
        public void unhandled_exception_is_propagated_via_event() {
            var task = new Task<int>();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => {
                Assert.AreEqual(ex, e.ExceptionObject);
                eventRaised++;
            };
            Task.UnhandledException += handler;
            try {
                task.Then(a => { throw ex; }).Done();
                task.Resolve(5);
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void exception_in_done_callback_is_propagated_via_event() {
            var task = new Task<int>();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => {
                Assert.AreEqual(ex, e.ExceptionObject);
                eventRaised++;
            };
            Task.UnhandledException += handler;
            try {
                task.Then(x => { throw ex; }).Done();
                task.Resolve(5);
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void handled_exception_is_not_propagated_via_event() {
            var task = new Task<int>();
            var ex = new Exception();
            var eventRaised = 0;
            EventHandler<UnhandledExceptionEventArgs> handler = (s, e) => ++eventRaised;
            Task.UnhandledException += handler;
            try {
                task.Then(a => { throw ex; }).Catch(_ => {
                    // Catch the error.
                }).Done();
                task.Resolve(5);
                Assert.AreEqual(1, eventRaised);
            } finally {
                Task.UnhandledException -= handler;
            }
        }

        [Test]
        public void can_handle_Done_onResolved() {
            var task = new Task<int>();
            var callback = 0;
            var expectedValue = 5;
            task.Then(value => {
                Assert.AreEqual(expectedValue, value);
                callback++;
            }).Done();
            task.Resolve(expectedValue);
            Assert.AreEqual(1, callback);
        }

        [Test]
        public void can_handle_Done_onResolved_with_onReject() {
            var task = new Task<int>();
            var callback = 0;
            var errorCallback = 0;
            var expectedValue = 5;
            task.Then(value => {
                Assert.AreEqual(expectedValue, value);
                callback++;
            }).Catch(ex => { ++errorCallback; }).Done();
            task.Resolve(expectedValue);
            Assert.AreEqual(1, callback);
            Assert.AreEqual(0, errorCallback);
        }

        /*todo:
         * Also want a test that exception thrown during Then triggers the error handler.
         * How do Javascript tasks work in this regard?
        [Test]
        public void exception_during_Done_onResolved_triggers_error_hander()
        {
            var task = new Task<int>();
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
            var task = new Task<int>();
            var callback = 0;
            var errorCallback = 0;
            var expectedException = new Exception();
            task.Then(value => { throw expectedException; }).Then(() => ++callback).Catch(ex => {
                Assert.AreEqual(expectedException, ex);
                errorCallback++;
            }).Done();
            task.Resolve(6);
            Assert.AreEqual(0, callback);
            Assert.AreEqual(1, errorCallback);
        }

    }

}
