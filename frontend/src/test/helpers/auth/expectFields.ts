// test/helpers/auth/expectFields.ts
export function expectFieldsEnabled({
  email,
  password,
  confirmPassword,
}: {
  email: () => HTMLElement;
  password: () => HTMLElement;
  confirmPassword: () => HTMLElement;
}) {
  expect(email()).not.toBeDisabled();
  expect(password()).not.toBeDisabled();
  expect(confirmPassword()).not.toBeDisabled();
}

export function expectFieldsDisabled({
  email,
  password,
  confirmPassword,
}: {
  email: () => HTMLElement;
  password: () => HTMLElement;
  confirmPassword: () => HTMLElement;
}) {
  expect(email()).toBeDisabled();
  expect(password()).toBeDisabled();
  expect(confirmPassword()).toBeDisabled();
}
