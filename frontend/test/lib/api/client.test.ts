// @vitest-environment node

import { describe, it, expect, vi, beforeEach } from 'vitest';
import { createApiClient } from '@/lib/api/client';

describe('ApiClient', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  describe('GET', () => {
    it('returns ok with data on successful response', async () => {
      const mockData = { id: 1, name: 'Buddy' };
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: true,
          status: 200,
          json: () => Promise.resolve(mockData),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.get('/dogs');

      expect(fetch).toHaveBeenCalledWith('http://localhost/dogs', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
      });
      expect(result).toEqual({ ok: true, data: mockData });
    });

    it('returns network error when fetch throws', async () => {
      vi.stubGlobal(
        'fetch',
        vi.fn().mockRejectedValue(new TypeError('Failed to fetch')),
      );

      const client = createApiClient('http://localhost');
      const result = await client.get('/dogs');

      expect(result).toEqual({
        ok: false,
        error: {
          type: 'network',
          message: 'Failed to fetch',
        },
      });
    });

    it('returns http error on non-ok response', async () => {
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: false,
          status: 500,
          json: () => Promise.resolve({ message: 'Internal Server Error' }),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.get('/dogs');

      expect(result).toEqual({
        ok: false,
        error: {
          type: 'http',
          status: 500,
          message: 'Internal Server Error',
        },
      });
    });

    it('returns validation error with field errors on 422', async () => {
      const body = {
        message: 'Validation failed',
        errors: { name: ['Name is required'], age: ['Must be positive'] },
      };
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: false,
          status: 422,
          json: () => Promise.resolve(body),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.get('/dogs');

      expect(result).toEqual({
        ok: false,
        error: {
          type: 'validation',
          status: 422,
          message: 'Validation failed',
          errors: { name: ['Name is required'], age: ['Must be positive'] },
        },
      });
    });

    // next test for GET would go here

  });

  describe('POST', () => {
    it('sends body and returns ok with data on success', async () => {
      const requestBody = { name: 'Buddy', breed: 'Labrador' };
      const responseData = { id: 1, name: 'Buddy', breed: 'Labrador' };
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: true,
          status: 201,
          json: () => Promise.resolve(responseData),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.post('/dogs', requestBody);

      expect(fetch).toHaveBeenCalledWith('http://localhost/dogs', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      });
      expect(result).toEqual({ ok: true, data: responseData });
    });

    // next test for POST would go here
  });

  describe('PUT', () => {
    it('sends body and returns ok with data on success', async () => {
      const requestBody = { name: 'Buddy Jr' };
      const responseData = { id: 1, name: 'Buddy Jr', breed: 'Labrador' };
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: true,
          status: 200,
          json: () => Promise.resolve(responseData),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.put('/dogs/1', requestBody);

      expect(fetch).toHaveBeenCalledWith('http://localhost/dogs/1', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      });
      expect(result).toEqual({ ok: true, data: responseData });
    });
  });

  describe('DELETE', () => {
    it('returns ok with data on success', async () => {
      vi.stubGlobal(
        'fetch',
        vi.fn().mockResolvedValue({
          ok: true,
          status: 200,
          json: () => Promise.resolve({ deleted: true }),
        }),
      );

      const client = createApiClient('http://localhost');
      const result = await client.delete('/dogs/1');

      expect(fetch).toHaveBeenCalledWith('http://localhost/dogs/1', {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
      });
      expect(result).toEqual({ ok: true, data: { deleted: true } });
    });
  });

});
