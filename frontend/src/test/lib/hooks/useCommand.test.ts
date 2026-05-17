// @vitest-environment jsdom
import { renderHook, act } from '@testing-library/react';
import { useCommand } from '@/lib/hooks/useCommand';

describe('useCommand', () => {
  it('starts with no errors and not submitting', () => {
    const { result } = renderHook(() =>
      useCommand(vi.fn(), vi.fn())
    );
    expect(result.current.errors).toBeUndefined();
    expect(result.current.isSubmitting).toBe(false);
  });

  it('calls onSuccess when the command succeeds', async () => {
    const commandFn = vi.fn().mockResolvedValue({ success: true });
    const onSuccess = vi.fn();

    const { result } = renderHook(() => useCommand(commandFn, onSuccess));

    await act(async () => {
      await result.current.handleSubmit({ name: 'Buddy' });
    });

    expect(commandFn).toHaveBeenCalledWith({ name: 'Buddy' });
    expect(onSuccess).toHaveBeenCalled();
  });

  it('sets errors and resets isSubmitting on failure', async () => {
    const commandFn = vi.fn().mockResolvedValue({
      success: false,
      errors: { name: 'Name is required' },
    });
    const onSuccess = vi.fn();

    const { result } = renderHook(() => useCommand(commandFn, onSuccess));

    await act(async () => {
      await result.current.handleSubmit({ name: '' });
    });

    expect(onSuccess).not.toHaveBeenCalled();
    expect(result.current.errors).toEqual({ name: 'Name is required' });
    expect(result.current.isSubmitting).toBe(false);
  });

  it('clears previous errors on new submit', async () => {
    const commandFn = vi.fn()
      .mockResolvedValueOnce({ success: false, errors: { name: 'Required' } })
      .mockResolvedValueOnce({ success: true });
    const onSuccess = vi.fn();

    const { result } = renderHook(() => useCommand(commandFn, onSuccess));

    await act(async () => {
      await result.current.handleSubmit({ name: '' });
    });
    expect(result.current.errors).toEqual({ name: 'Required' });

    await act(async () => {
      await result.current.handleSubmit({ name: 'Buddy' });
    });
    expect(result.current.errors).toBeUndefined();
    expect(onSuccess).toHaveBeenCalled();
  });
});