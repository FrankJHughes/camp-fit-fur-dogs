import type { UserEvent } from "@testing-library/user-event";
import { selectors } from "./selectors";

export async function fillValidCreateAccountForm(user: UserEvent) {
  await user.type(selectors.email(), "frank@example.com");
  await user.type(selectors.password(), "Password123!");
  await user.type(selectors.confirmPassword(), "Password123!");
}
