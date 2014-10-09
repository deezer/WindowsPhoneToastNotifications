using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WindowsPhoneToastNotifications.Test
{
    /// <summary>
    /// DispatcherExtensions helps run Unit tests on UI controls.
    /// More info at : 
    // http://www.pedrolamas.com/2013/03/25/windows-phone-8-unit-testing-in-the-ui-thread-with-vs-2012-2-ctp4/
    // https://gist.github.com/JakeGinnivan/5219390
    // We-ve tested the solution (custom test attribute) from Pedro Lamas, without success.
    // MS Test runner does not detect the test.
    /// </summary>
    public static class DispatcherExtensions
    {
        public static Task<T> InvokeTaskAsync<T>(this Dispatcher dispatcher, Func<Task<T>> func)
        {
            var tcs = new TaskCompletionSource<T>();
            dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    var result = await func();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }));

            return tcs.Task;
        }

        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }));

            return tcs.Task;
        }
        public static Task InvokeAsync(this Dispatcher dispatcher, Func<object> func)
        {
            var tcs = new TaskCompletionSource<object>();
            dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }));

            return tcs.Task;
        }
    }
}