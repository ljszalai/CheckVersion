import os
import sys
import re


class ConvertVersion:

    def __init__(self):
        self._tool_path = os.path.join(os.path.dirname(__file__), 'GetFVer.exe')
        self._filename = ''
        self._fileversion = ''
        self._productversion = ''
        self._status = ''

    def get_fullverstring(self, filename):
        self._filename = filename
        self._run_command('GetFVer.exe', filename)
        return self._status

    def get_version_string(self, filename, pattern, separator):
        result = ''
        if self._filename != filename:
            self.get_fullverstring(filename)
        processedPattern = re.compile(pattern)
        for line in self._status.splitlines():
            if re.search(pattern, line):
                valueList = processedPattern.search(line).groups()
                for val in valueList:
                    result = result + val + separator
        if len(result) > 0:
            result = result[:-1]
        return result                 
                

    def _run_command(self, command, *args):
        command = '"%s" %s' % (self._tool_path, ' '.join(args))
        process = os.popen(command)
        self._status = process.read().strip()
        process.close()
