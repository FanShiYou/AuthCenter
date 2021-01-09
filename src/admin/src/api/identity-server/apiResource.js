import request from '@/utils/request'
import { transformAbpListQuery } from '@/utils/abp'

export function getApiResources(query) {
  return request({
    url: '/api/app/apiResource',
    method: 'get',
    params: transformAbpListQuery(query)
  })
}

export function createApiResource(data) {
  return request({
    url: '/api/app/apiResource',
    method: 'post',
    data
  })
}

export function updateApiResource(data) {
  return request({
    url: `/api/app/apiResource/${data.id}`,
    method: 'put',
    data
  })
}

export function deleteApiResource(id) {
  return request({
    url: `/api/app/apiResource/${id}`,
    method: 'delete'
  })
}

export function getApiResource(id) {
  return request({
    url: `/api/app/apiResource/${id}`,
    method: 'get'
  })
}

export function getApiResourceScope(id) {
  return request({
    url: `/api/app/apiResource/${id}/scopes`,
    method: 'get'
  })
}

export function addApiResourceScope(data) {
  return request({
    url: `/api/app/apiResource/scopes`,
    method: 'post',
    data
  })
}

export function deleteApiResourceScope(id, name) {
  return request({
    url: `/api/app/apiResource/${id}/scopes?name=${name}`,
    method: 'delete'
  })
}

export function updateApiResourceScope(beforeApiResourceId, beforeName, scope) {
  return request({
    url: `/api/app/apiResource/scopes`,
    method: 'put',
    data: { 
      beforeApiResourceId, 
      beforeName, 
      scope
    }
  })
}