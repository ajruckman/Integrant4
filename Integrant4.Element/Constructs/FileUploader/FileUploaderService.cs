using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Integrant4.Element.Constructs.FileUploader
{
    public class FileUploaderService
    {
        private readonly ConcurrentDictionary<Guid, Action<FileUploader.File>> _addListeners = new();
        private readonly ConcurrentDictionary<Guid, Action<FileUploader.File>> _remListeners = new();

        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<int, FileUploader.File>>
            _fileMap = new();

        private ushort _serialID;

        internal void Subscribe(Guid uid, Action<FileUploader.File> addListener, Action<FileUploader.File> remListener)
        {
            _addListeners[uid] = addListener;
            _remListeners[uid] = remListener;
            _fileMap[uid]      = new ConcurrentDictionary<int, FileUploader.File>();
        }

        internal void Unsubscribe(Guid guid)
        {
            _addListeners.Remove(guid, out _);
            _remListeners.Remove(guid, out _);

            foreach ((_, FileUploader.File file) in _fileMap[guid])
            {
                file.Data.Dispose();
            }

            _fileMap[guid].Clear();
            _fileMap.Remove(guid, out _);
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        public FileUploader.File? Get(Guid guid, int serialID)
        {
            if (!_fileMap.TryGetValue(guid, out var files))
                return null;

            if (!files.TryGetValue(serialID, out var file))
                return null;

            return file;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public IReadOnlyList<FileUploader.File> List(Guid guid)
        {
            return _fileMap[guid].Values.OrderBy(v => v.SerialID).ToArray();
        }

        public void Add(Guid guid, string name, MemoryStream data)
        {
            ushort id = _serialID++;

            var file = new FileUploader.File(id, name, data);

            _fileMap[guid][id] = file;

            if (_addListeners.TryGetValue(guid, out Action<FileUploader.File>? listener))
            {
                listener.Invoke(file);
            }
        }

        public void Remove(Guid guid, int serial)
        {
            _fileMap[guid].Remove(serial, out FileUploader.File? file);
            
            if (file != null && _remListeners.TryGetValue(guid, out Action<FileUploader.File>? listener))
            {
                listener.Invoke(file);
            }
        }
    }

    public static class FileUploaderServiceExtensions
    {
        public static IServiceCollection AddFileUploaderService(this IServiceCollection services) =>
            services.AddSingleton<FileUploaderService>();
    }
}