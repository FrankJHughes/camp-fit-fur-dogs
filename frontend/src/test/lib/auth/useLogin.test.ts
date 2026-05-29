import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { useLogin } from "@/lib/auth/useLogin";

// Mock initiateLogin
const initiateLoginMock = vi.fn();

vi.mock("@/lib/auth/login", () => ({
  initiateLogin: (...args: any[]) => initiateLoginMock(...args),
}));

describe("useLogin", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
    initiateLoginMock.mockReset();
  });

  it("starts with loading = false", () => {
    const { result } = renderHook(() => useLogin());
    expect(result.current.loading).toBe(false);
  });

  it("sets loading = true when login() is called", async () => {
    initiateLoginMock.mockResolvedValue({
      redirectUrl: "https://auth0.com",
    });

    const { result } = renderHook(() => useLogin());

    await act(async () => {
      await result.current.login();
    });

    expect(result.current.loading).toBe(true);
  });

  it("calls initiateLogin() when login() is invoked", async () => {
    initiateLoginMock.mockResolvedValue({
      redirectUrl: "https://auth0.com",
    });

    const { result } = renderHook(() => useLogin());

    await act(async () => {
      await result.current.login();
    });

    expect(initiateLoginMock).toHaveBeenCalledTimes(1);
  });

  it("redirects when initiateLogin returns a redirectUrl", async () => {
    const assignMock = vi.fn();
    vi.stubGlobal("window", {
      location: { assign: assignMock },
    });

    initiateLoginMock.mockResolvedValue({
      redirectUrl: "https://auth0.com",
    });

    const { result } = renderHook(() => useLogin());

    await act(async () => {
      await result.current.login();
    });

    expect(assignMock).toHaveBeenCalledWith("https://auth0.com");
  });

  it("throws if initiateLogin throws", async () => {
    initiateLoginMock.mockRejectedValue(new Error("boom"));

    const { result } = renderHook(() => useLogin());

    await expect(
      act(async () => {
        await result.current.login();
      })
    ).rejects.toThrow("boom");
  });
});
