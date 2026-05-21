// test/helpers/mockRouter.ts
import { vi } from "vitest";

export const pushMock = vi.fn();

// Route params you can mutate in tests
export const paramsMock: Record<string, string> = {};

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: pushMock }),
  useParams: () => paramsMock,
}));
