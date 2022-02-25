'use strict';

export async function fileStream (fileInputElementId) {     
  let fileInputElement =  document.getElementById(fileInputElementId);
  let data = new FormData();
  data.append('File', fileInputElement.files[0])
  let status = await AJAXSubmit('/fileupload/stream', data).then(response => response.status);
  return { statusCode : status};
}

export async function AJAXSubmit (url, data) {
    return await fetch(url, {
        method: 'POST',
        body: data
    });
}