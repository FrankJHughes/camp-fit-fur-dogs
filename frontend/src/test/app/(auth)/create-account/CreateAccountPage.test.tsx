import { vi } from "vitest";
import { pushMock } from "@/test/helpers/auth/mockRouter";
import { setupCreateAccount } from "@/test/helpers/auth/setupCreateAccount";
import { fillValidCreateAccountForm } from "@/test/helpers/auth/fillValidCreateAccountForm";
import { fakeAsyncSubmit } from "@/test/helpers/auth/fakeSubmit";
import { expectFieldsEnabled, expectFieldsDisabled } from "@/test/helpers/auth/expectFields";
import { selectors } from "@/test/helpers/auth/selectors";

describe("CreateAccountPage — Slice 1", () => {
  beforeEach(() => {
    pushMock.mockClear();
  });

  it("renders the create account form fields", () => {
    const ctx = setupCreateAccount();

    expect(ctx.email()).toBeInTheDocument();
    expect(ctx.password()).toBeInTheDocument();
    expect(ctx.confirmPassword()).toBeInTheDocument();
    expect(ctx.button()).toBeInTheDocument();
  });

  it("submits form values through React Hook Form", async () => {
    const handleSubmit = vi.fn();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);
    await ctx.user.click(ctx.button());

    await vi.waitFor(() =>
      expect(handleSubmit).toHaveBeenCalledWith({
        email: "frank@example.com",
        password: "Password123!",
        confirmPassword: "Password123!",
      })
    );
  });

  it("shows validation errors when submitting an empty form", async () => {
    const ctx = setupCreateAccount();

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(selectors.emailError()).toBeInTheDocument());
    await vi.waitFor(() => expect(selectors.passwordError()).toBeInTheDocument());
    await vi.waitFor(() => expect(selectors.confirmPasswordError()).toBeInTheDocument());
  });

  it("shows an error when the email is invalid", async () => {
    const ctx = setupCreateAccount();

    await ctx.user.type(ctx.email(), "not-an-email");
    await ctx.user.type(ctx.password(), "Password123!");
    await ctx.user.type(ctx.confirmPassword(), "Password123!");

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(selectors.invalidEmailError()).toBeInTheDocument());
  });

  it("shows an error when the password is too short", async () => {
    const ctx = setupCreateAccount();

    await ctx.user.type(ctx.email(), "frank@example.com");
    await ctx.user.type(ctx.password(), "123");
    await ctx.user.type(ctx.confirmPassword(), "123");

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(selectors.shortPasswordError()).toBeInTheDocument());
  });

  it("shows an error when the passwords do not match", async () => {
    const ctx = setupCreateAccount();

    await ctx.user.type(ctx.email(), "frank@example.com");
    await ctx.user.type(ctx.password(), "Password123!");
    await ctx.user.type(ctx.confirmPassword(), "Different123!");

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(selectors.passwordMismatchError()).toBeInTheDocument());
  });

  it("disables the submit button while the form is submitting", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);

    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(ctx.button()).toBeDisabled());
    await vi.waitFor(() => expect(handleSubmit).toHaveBeenCalled());
    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());
  });

  it("clears validation errors after fixing the inputs", async () => {
    const ctx = setupCreateAccount();

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(selectors.emailError()).toBeInTheDocument());
    await vi.waitFor(() => expect(selectors.passwordError()).toBeInTheDocument());
    await vi.waitFor(() => expect(selectors.confirmPasswordError()).toBeInTheDocument());

    await fillValidCreateAccountForm(ctx.user);

    await vi.waitFor(() => {
      expect(selectors.emailError()).toBeNull();
      expect(selectors.passwordError()).toBeNull();
      expect(selectors.confirmPasswordError()).toBeNull();
    });
  });

  it("disables all fields while the form is submitting", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);

    expectFieldsEnabled(ctx);

    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expectFieldsDisabled(ctx));
    await vi.waitFor(() => expect(handleSubmit).toHaveBeenCalled());
    await vi.waitFor(() => expectFieldsEnabled(ctx));
  });

  it("prevents double submission while the form is submitting", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);

    const button = ctx.button();
    await Promise.all([
      ctx.user.click(button),
      ctx.user.click(button),
      ctx.user.click(button),
    ]);

    expect(handleSubmit).toHaveBeenCalledTimes(1);

    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());
  });

  it("shows a success message after successful submission", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);
    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(handleSubmit).toHaveBeenCalled());
    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());

    // Flush one more microtask tick so success message can render
    await vi.waitFor(() => { });

    await vi.waitFor(() => expect(selectors.successMessage()).toBeInTheDocument());
  });

  it("clears the success message when the user edits any field after success", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);
    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(handleSubmit).toHaveBeenCalled());
    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());

    // Flush microtask so success message appears
    await vi.waitFor(() => { });

    await vi.waitFor(() => expect(selectors.successMessage()).toBeInTheDocument());

    await ctx.user.type(ctx.email(), "x");

    await vi.waitFor(() => {
      expect(selectors.successMessage()).toBeNull();
    });
  });

  it("redirects to the success page after successful submission", async () => {
    const handleSubmit = fakeAsyncSubmit();
    const ctx = setupCreateAccount({ onSubmit: handleSubmit });

    await fillValidCreateAccountForm(ctx.user);
    await ctx.user.click(ctx.button());

    await vi.waitFor(() => expect(handleSubmit).toHaveBeenCalled());
    await vi.waitFor(() => expect(ctx.button()).not.toBeDisabled());

    // Flush microtask so redirect happens
    await vi.waitFor(() => { });

    expect(pushMock).toHaveBeenCalledWith("/owners/create-account/success");
  });
});
