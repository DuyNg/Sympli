// src/api/searchApi.ts
import { apiClient } from './axiosInstance';
import { SearchRequest, SearchResult } from '../models';

export const searchKeyword = async (payload: SearchRequest): Promise<SearchResult> => {
  const url = `search/${encodeURIComponent(payload.browser)}/${encodeURIComponent(payload.keyword)}/${encodeURIComponent(payload.url)}`;
  const response = await apiClient.get<SearchResult>(url);
  return response.data;
};

