import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import LoginPage from "@/app/(auth)/login/page";

// --- Mock useLogin hook ------------------------------------------------------
// We define a mutable mock object that the mock factory returns.
// Tests can mutate this object to simulate different hook states.

const hookState = {
  login: vi.fn(),
  loading: false,
};

vi.mock("@/lib/auth/useLogin", () => {
  return {
    useLogin: () => hookState,
  };
});
// -----------------------------------------------------------------------------

describe("LoginPage", () => {
  beforeEach(() => {
    hookState.login.mockReset();
    hookState.loading = false;
  });

  it("renders a Login heading", () => {
    render(<LoginPage />);
    expect(screen.getByRole("heading", { name: /login/i })).toBeInTheDocument();
  });

  it("renders a Login button", () => {
    render(<LoginPage />);
    expect(screen.getByRole("button", { name: /login/i })).toBeInTheDocument();
  });

  it("calls login() when the Login button is clicked", async () => {
    render(<LoginPage />);

    const button = screen.getByRole("button", { name: /login/i });
    await userEvent.click(button);

    expect(hookState.login).toHaveBeenCalledTimes(1);
  });

  it("displays 'Redirecting…' and disables the button when loading is true", () => {
    // Simulate loading state
    hookState.loading = true;

    render(<LoginPage />);

    const button = screen.getByRole("button", { name: /redirecting/i });
    expect(button).toBeDisabled();
  });
});
