// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Test
{
    using Microsoft.Extensions.Primitives;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class TestHelper.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Mocks the command for the test context.  A successful result is returned unless
        /// there is an exception thrown.
        ///
        /// It's important to understand that this context is lost once the process
        /// executing the thread executes somewhere else.
        ///
        /// For example, if the test is an integration test, this test context will
        /// no longer be in play.  For integration tests where you are hopping to
        /// another context, you have to mock the behavior globally using
        /// <see cref="MockCommandGlobally{TRequest}(Action{TRequest})"/>
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse
        public static void MockCommandForTestDuration<TRequest>(Action<TRequest> f)
            where TRequest : class, IReturn<CommandResponse> =>
                MockCommand.SetForTestScope(f);

        /// <summary>
        /// Mocks the command globally.  This is active for all threads across all tests.
        /// This is only used when there is a separate process and the <see cref="AsyncLocal{T}"/>
        /// can't track the same thread across the unit test.
        ///
        /// For example: If you have an integration test that is calling a web service, once
        /// the service is called, the test context is no longer available and your mocks can only
        /// be hit if you set the mock globally.
        ///
        /// If you use this method, you can not run tests in parallel because they will all share
        /// this same setting for their mocked behavior.
        ///
        /// Most of the time, you'll want to use
        /// <see cref="MockCommandForTestDuration{TRequest}(Action{TRequest})"/>
        /// so that you can run tests in parallel.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t argument.</typeparam>
        /// <typeparam name="TResponse">The type of the t result.</typeparam>
        /// <param name="f">The f.</param>
        public static void MockCommandGlobally<TRequest>(Action<TRequest> f)
            where TRequest : class, IReturn<CommandResponse> =>
                MockRequest<TRequest, CommandResponse>.SetGlobally(r =>
                {
                    try
                    {
                        f(r); return CommandResponse.Success;
                    }
                    catch (Exception ex)
                    {
                        return CommandResponse.Failure(ex.Message);
                    }
                });

        /// <summary>
        /// Mocks the request for the test context.
        ///
        /// It's important to understand that this context is lost once the process
        /// executing the thread executes somewhere else.
        ///
        /// For example, if the test is an integration test, this test context will
        /// no longer be in play.  For integration tests where you are hopping to
        /// another context, you have to mock the behavior globally using
        /// <see cref="MockRequestGlobally{TRequest, TResponse}(Func{TRequest, TResponse})"/>
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="f"></param>
        public static void MockRequestForTestDuration<TRequest, TResponse>(Func<TRequest, TResponse> f)
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveRequestStatus =>
            MockRequest<TRequest, TResponse>.SetForTestScope(f);

        /// <summary>
        /// Mocks the request globally.  This is active for all threads across all tests.
        /// This is only used when there is a separate process and the <see cref="AsyncLocal{T}"/>
        /// can't track the same thread across the unit test.
        ///
        /// For example: If you have an integration test that is calling a web service, once
        /// the service is called, the test context is no longer available and your mocks can only
        /// be hit if you set the mock globally.
        ///
        /// If you use this method, you can not run tests in parallel because they will all share
        /// this same setting for their mocked behavior.
        ///
        /// Most of the time, you'll want to use <see cref="MockRequestForTestDuration{TRequest, TResult}(Func{TRequest, TResult})"/>
        /// so that you can run tests in parallel.
        /// </summary>
        /// <typeparam name="TRequest">The type of the t argument.</typeparam>
        /// <typeparam name="TResponse">The type of the t result.</typeparam>
        /// <param name="f">The f.</param>
        public static void MockRequestGlobally<TRequest, TResponse>(Func<TRequest, TResponse> f)
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveRequestStatus =>
            MockRequest<TRequest, TResponse>.SetGlobally(f);

        public static void OnlyLoadReferencedAssemblies(bool value = true) =>
            TypeRegistry.LoadFromDisk = !true;

        /// <summary>
        /// Sets the cache result for duration of the test.
        /// <para>
        /// Calling this method will register the following three services:</para><para><font color="#2a2a2a">IKnownCache&lt;T&gt;<br /><font color="#2a2a2a">ICache&lt;T&gt;<br /><font color="#2a2a2a">ICacheFactory</font></font></font></para><para>
        /// For calls to <see cref="IKnownCache{TValue}" />, the key will be <see cref="string.Empty" /></para><para>
        /// This is how the class is used in a unit test:</para>
        /// <span id="cbc_1" codelanguage="CSharp" x-lang="CSharp">
        /// <div class="highlight-title"><span tabindex="0" class="highlight-copycode"></span>C#</div>
        /// <div class="code"><pre xml:space="preserve"><para>[Fact]<br />public async Task MyTestMethod()<br />{</para><para>    var cacheValue = "cached value";</para><para>    // use a function to evaluate the key and return what you want<br />    TestHelper.SetCacheResultForTestDuration&lt;string&gt;(key =&gt; cacheValue);  </para><para>}<br /></para></pre></div></span></summary>
        /// <typeparam name="TCachedItem">The type of the cached item.</typeparam>
        /// <param name="func">The function used to generate the desired result</param>
        //public static void SetCacheResultForTestDuration<TCachedItem>(Func<string, TCachedItem> func) =>
        //    MockCache.Set(func);        


        public static void SetRequestContext(ServiceType commandType) =>
            RequestContext.Current =
                new RequestContext(
                    currentServiceType    : commandType,
                    sessionId             : SequentialGuid.NewGuid(),
                    conversationId        : SequentialGuid.NewGuid(),
                    appContext            : new AppContextKey(SequentialGuid.NewGuid(), 0,0,1),
                    requestCallCount      : RequestContext.Current.RequestCallCount,
                    decoratorCallCount    : RequestContext.Current.DecoratorCallCount,
                    metadata              : RequestContext.Current.Metadata);

        public static void SetRequestContextMetadata(IDictionary<string, StringValues> md) =>
            RequestContext.Current =
                new RequestContext(
                    currentServiceType    : RequestContext.Current.ServiceType,
                    sessionId             : SequentialGuid.NewGuid(),
                    conversationId        : SequentialGuid.NewGuid(),
                    appContext            : new AppContextKey(SequentialGuid.NewGuid(), 0, 0, 1),
                    requestCallCount      : RequestContext.Current.RequestCallCount,
                    decoratorCallCount    : RequestContext.Current.DecoratorCallCount,
                    metadata              : new ReadOnlyDictionary<string, StringValues>(md));
    }
}