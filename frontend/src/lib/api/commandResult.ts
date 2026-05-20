export interface CommandResult {
  success: boolean;
  error?: string;
  errors?: Record<string, string>;
}
