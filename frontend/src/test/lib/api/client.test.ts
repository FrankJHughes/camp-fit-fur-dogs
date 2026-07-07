import { describe, it, expect, vi, beforeEach } from 'vitest';

// Disable global ApiClient mock from setup.ts
vi.unmock('@/lib/api/client');

// Mock fetch BEFORE importing ApiClient
const fetchMock = vi.fn();
globalThis.fetch = fetchMock as any;

import { createApiClient } from '@/lib/api/client';

type ApiClientType = ReturnType<typeof createApiClient>;

describe('ApiClient', () => {
  let client: ApiClientType;

  beforeEach(() => {
    fetchMock.mockReset();
    client = createApiClient('http://localhost');
  });

  it('GET returns ok with data on successful response', async () => {
    fetchMock.mockResolvedValue({
      ok: true,
      json: async () => ({ dogs: [] }),
    });

    const result = await client.get('/dogs');

    expect(fetchMock).toHaveBeenCalledWith('http://localhost/dogs', {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
    });

    expect(result).toEqual({ ok: true, data: { dogs: [] } });
  });

  it('GET returns http error on non-ok response', async () => {
    fetchMock.mockResolvedValue({
      ok: false,
      status: 500,
      statusText: 'Internal Server Error',
      json: async () => ({ message: 'Internal Server Error' }),
    });

    const result = await client.get('/dogs');

    expect(result).toEqual({
      ok: false,
      error: {
        type: 'http',
        status: 500,
        message: 'Internal Server Error', // ← corrected
      },
    });
  });

  it('POST sends body and returns ok with data on success', async () => {
    fetchMock.mockResolvedValue({
      ok: true,
      json: async () => ({ id: 1 }),
    });

    const body = { name: 'Buddy', breed: 'Labrador' };
    const result = await client.post('/dogs', body);

    expect(fetchMock).toHaveBeenCalledWith('http://localhost/dogs', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(body),
    });

    expect(result).toEqual({ ok: true, data: { id: 1 } });
  });

  it('PUT sends body and returns ok with data on success', async () => {
    fetchMock.mockResolvedValue({
      ok: true,
      json: async () => ({ id: 1 }),
    });

    const body = { name: 'Buddy Jr' };
    const result = await client.put('/dogs/1', body);

    expect(fetchMock).toHaveBeenCalledWith('http://localhost/dogs/1', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(body),
    });

    expect(result).toEqual({ ok: true, data: { id: 1 } });
  });

  it('DELETE returns ok with data on success', async () => {
    fetchMock.mockResolvedValue({
      ok: true,
      json: async () => ({ success: true }),
    });

    const result = await client.delete('/dogs/1');

    expect(fetchMock).toHaveBeenCalledWith('http://localhost/dogs/1', {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
    });

    expect(result).toEqual({ ok: true, data: { success: true } });
  });

  it('returns validation error on 422', async () => {
    fetchMock.mockResolvedValue({
      ok: false,
      status: 422,
      json: async () => ({
        message: 'Validation failed',
        errors: { name: ['Required'] },
      }),
    });

    const result = await client.post('/dogs', {});

    expect(result).toEqual({
      ok: false,
      error: {
        type: 'validation',
        status: 422,
        message: 'Validation failed',
        errors: { name: ['Required'] },
      },
    });
  });

  it('returns network error on thrown fetch', async () => {
    fetchMock.mockRejectedValue(new Error('boom'));

    const result = await client.get('/dogs');

    expect(result).toEqual({
      ok: false,
      error: {
        type: 'network',
        message: 'boom',
      },
    });
  });
});
