import React from 'react'
import Editor from '@monaco-editor/react';
import { Button } from './ui/button';

interface TextEditorProps {
  code: string;
  setCode: (code: string) => void;
  handleExecute: () => void;
}

const TextEditor = ({ code, setCode, handleExecute}: TextEditorProps) => {

  const handleCreate = () => {
    console.log('create file');
  }

  const openFile = () => {
    console.log('open file');
  }

  const saveFile = () => {
    console.log('save file');
  }

  return (
    <div className='flex flex-col items-center justify-center bg-[#131313]  w-2/3 h-full'>
      <div className='flex gap-2 border-b-2 border-gray-800 p-2 w-full'>
        <Button className='bg-[#267dff]' onClick={handleExecute}>Run</Button>
        <Button onClick={() => setCode('')}>Clear</Button>
        <Button onClick={handleCreate}>Create</Button>
        <Button onClick={openFile}>Open</Button>
        <Button onClick={saveFile}>Save</Button>
      </div>
      <Editor
        height="80%"
        defaultLanguage="go"
        theme='vs-dark'
        value={code}
        onChange={(value) => value && setCode(value)}
        
    />
    </div>
    

  )
}

export default TextEditor