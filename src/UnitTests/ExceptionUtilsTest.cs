// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.CompilerServices;

#if NETFRAMEWORK
using NanoByte.Common.Native;
#endif

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="ExceptionUtils"/>.
/// </summary>
public class ExceptionUtilsTest
{
    [Fact]
    public void TestGetMessageWithInner()
        => new Exception("Message 1", new Exception("Message 1", new Exception("Message 2")))
          .GetMessageWithInner().Should().Be($"Message 1{Environment.NewLine}Message 2");

    [Fact]
    public void TestPreserveStack()
    {
        Exception caught = default!;
        try
        {
            ThrowMockException();
        }
        catch (Exception ex)
        {
            caught = ex;
        }

        var exceptionAssertion = caught.Invoking(x => x.Rethrow())
                                       .Should().Throw<InvalidOperationException>();
        exceptionAssertion.WithMessage("Test exception");

        // Preserving the stack trace is only possible on .NET Framework on Windows
#if NETFRAMEWORK
        if (WindowsUtils.IsWindows)
            exceptionAssertion.Where(x => x.StackTrace.Contains("ThrowMockException"));
#endif
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowMockException() => throw new InvalidOperationException("Test exception");

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.ApplyWithRollback{T}"/> correctly performs rollbacks on exceptions.
    /// </summary>
    [Fact]
    public void TestApplyWithRollback()
    {
        var applyCalledFor = new List<int>();
        var rollbackCalledFor = new List<int>();
        new Action(() => new[] {1, 2, 3}.ApplyWithRollback(
                apply: value =>
                {
                    applyCalledFor.Add(value);
                    if (value == 2) throw new ArgumentException("Test exception");
                },
                rollback: rollbackCalledFor.Add))
           .Should().Throw<ArgumentException>(because: "Exceptions should be passed through after rollback.");

        applyCalledFor.Should().Equal(1, 2);
        rollbackCalledFor.Should().Equal(2, 1);
    }

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.TryAny{T}"/> correctly handles fail conditions followed by success conditions.
    /// </summary>
    [Fact]
    public void TestTryAnySucceed()
    {
        var actionCalledFor = new List<int>();
        new[] {1, 2, 3}.TryAny(value =>
        {
            actionCalledFor.Add(value);
            if (value == 1) throw new ArgumentException("Test exception");
        });

        actionCalledFor.Should().Equal(1, 2);
    }

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.TryAny{T}"/> correctly handles pure fail conditions.
    /// </summary>
    [Fact]
    public void TestTryAnyFail()
    {
        var actionCalledFor = new List<int>();
        new Action(() => new[] {1, 2, 3}.TryAny(value =>
            {
                actionCalledFor.Add(value);
                throw new ArgumentException("Test exception");
            }))
           .Should().Throw<ArgumentException>(because: "Last exceptions should be passed through.");

        actionCalledFor.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void TestRetryPassOnLastAttempt()
        => ExceptionUtils.Retry<InvalidOperationException>(lastAttempt =>
        {
            if (!lastAttempt) throw new InvalidOperationException("Test exception");
        });

    [Fact]
    public void TestRetryDoubleFail()
        => Assert.Throws<InvalidOperationException>(() => ExceptionUtils.Retry<InvalidOperationException>(
            delegate { throw new InvalidOperationException("Test exception"); }, maxRetries: 1));

    [Fact]
    public void TestRetryOtherExceptionType()
        => Assert.Throws<IOException>(() => ExceptionUtils.Retry<InvalidOperationException>(
            delegate { throw new IOException("Test exception"); }, maxRetries: 1));

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.ApplyWithRollbackAsync{T}"/> correctly performs rollbacks on exceptions.
    /// </summary>
    [Fact]
    public async Task TestApplyWithRollbackAsync()
    {
        var applyCalledFor = new List<int>();
        var rollbackCalledFor = new List<int>();
        await new Func<Task>(async () => await new[] {1, 2, 3}.ApplyWithRollbackAsync(
            apply: async value =>
            {
                await Task.Yield();
                applyCalledFor.Add(value);
                if (value == 2) throw new ArgumentException("Test exception");
            }, rollback: async x =>
            {
                await Task.Yield();
                rollbackCalledFor.Add(x);
            })).Should().ThrowAsync<ArgumentException>(because: "Exceptions should be passed through after rollback.");

        applyCalledFor.Should().Equal(1, 2);
        rollbackCalledFor.Should().Equal(2, 1);
    }

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.TryAnyAsync{T}"/> correctly handles fail conditions followed by success conditions.
    /// </summary>
    [Fact]
    public async Task TestTryAnyAsyncSucceed()
    {
        var actionCalledFor = new List<int>();
        await new[] {1, 2, 3}.TryAnyAsync(async value =>
        {
            await Task.Yield();
            actionCalledFor.Add(value);
            if (value == 1) throw new ArgumentException("Test exception");
        });

        actionCalledFor.Should().Equal(1, 2);
    }

    /// <summary>
    /// Ensures that <see cref="ExceptionUtils.TryAnyAsync{T}"/> correctly handles pure fail conditions.
    /// </summary>
    [Fact]
    public async Task TestTryAnyAsyncFail()
    {
        var actionCalledFor = new List<int>();
        await new Func<Task>(async () => await new[] {1, 2, 3}.TryAnyAsync(async value =>
        {
            await Task.Yield();
            actionCalledFor.Add(value);
            throw new ArgumentException("Test exception");
        })).Should().ThrowAsync<ArgumentException>(because: "Last exceptions should be passed through.");

        actionCalledFor.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task TestRetryAsyncPassOnLastAttempt() => await ExceptionUtils.RetryAsync<InvalidOperationException>(async lastAttempt =>
    {
        await Task.Yield();
        if (!lastAttempt) throw new InvalidOperationException("Test exception");
    });

    [Fact]
    public async Task TestRetryAsyncDoubleFail()
    {
        await new Func<Task>(async () => await ExceptionUtils.RetryAsync<InvalidOperationException>(
            async delegate
            {
                await Task.Yield();
                throw new InvalidOperationException("Test exception");
            }, maxRetries: 1)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task TestRetryAsyncOtherExceptionType()
    {
        await new Func<Task>(async () => await ExceptionUtils.RetryAsync<InvalidOperationException>(
            async delegate
            {
                await Task.Yield();
                throw new IOException("Test exception");
            }, maxRetries: 1)).Should().ThrowAsync<IOException>();
    }
}
