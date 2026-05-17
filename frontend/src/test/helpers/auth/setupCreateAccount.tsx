// test/helpers/auth/setupCreateAccount.ts
import { render } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { selectors } from "./selectors";
import CreateAccountPage from "@/app/(auth)/create-account/CreateAccountPage";

export function setupCreateAccount(props = {}) {
  const user = userEvent.setup();

  render(<CreateAccountPage {...props} />);

  return {
    user,
    email: () => selectors.email(),
    password: () => selectors.password(),
    confirmPassword: () => selectors.confirmPassword(),
    button: () => selectors.button(),
  };
}
