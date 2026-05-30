import { describe, it, expect, vi, beforeEach } from "vitest";
import { initiateLogin } from "@/lib/auth/login";

describe("initiateLogin", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it("calls /api/auth/login with GET and credentials: include", async () => {
    const mockResponse = {
      ok: true,
      json: () => Promise.resolve({ redirectUrl: "https://auth0.com" }),
    };

    const fetchMock = vi
      .spyOn(globalThis, "fetch")
      .mockResolvedValue(mockResponse as any);

    await initiateLogin();

    expect(fetchMock).toHaveBeenCalledWith("/api/auth/login", {
      method: "GET",
      credentials: "include",
    });
  });

  it("returns the redirectUrl when the response is valid", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ redirectUrl: "https://auth0.com" }),
    } as any);

    const result = await initiateLogin();

    expect(result.redirectUrl).toBe("https://auth0.com");
  });

  it("throws when the response is not ok", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      status: 500,
    } as any);

    await expect(initiateLogin()).rejects.toThrow(
      "Login initiation failed: 500"
    );
  });

  it("throws when redirectUrl is missing", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({}),
    } as any);

    await expect(initiateLogin()).rejects.toThrow(
      "Login initiation response missing redirectUrl"
    );
  });
});
