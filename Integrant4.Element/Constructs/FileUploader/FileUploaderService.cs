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
        private readonly ConcurrentDictionary<Guid, bool>            _multiple     = new();
        private readonly ConcurrentDictionary<Guid, HashSet<string>> _hashes       = new();
        private readonly ConcurrentDictionary<Guid, Action<File>>    _addListeners = new();
        private readonly ConcurrentDictionary<Guid, Action<File>>    _remListeners = new();

        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<int, File>>
            _fileMap = new();

        private ushort _serialID;

        internal void Subscribe
        (
            Guid         guid,
            bool         multiple,
            Action<File> addListener,
            Action<File> remListener
        )
        {
            _multiple[guid]     = multiple;
            _hashes[guid]       = new HashSet<string>();
            _addListeners[guid] = addListener;
            _remListeners[guid] = remListener;
            _fileMap[guid]      = new ConcurrentDictionary<int, File>();
        }

        internal void Unsubscribe(Guid guid)
        {
            _multiple.Remove(guid, out _);
            _hashes.Remove(guid, out _);
            _addListeners.Remove(guid, out _);
            _remListeners.Remove(guid, out _);

            foreach ((_, File file) in _fileMap[guid])
            {
                file.Data.Dispose();
            }

            _fileMap[guid].Clear();
            _fileMap.Remove(guid, out _);
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        public File? Get(Guid guid, int serialID)
        {
            if (!_fileMap.TryGetValue(guid, out var files))
                return null;

            if (!files.TryGetValue(serialID, out var file))
                return null;

            return file;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public IReadOnlyList<File> List(Guid guid)
        {
            return _fileMap[guid].Values.OrderBy(v => v.SerialID).ToArray();
        }

        public void Add(Guid guid, string name, MemoryStream data, string hash)
        {
            if (_hashes[guid].Contains(hash)) return;
            _hashes[guid].Add(hash);

            ushort id = _serialID++;

            var file = new File(id, name, data, hash);

            bool multiple = _multiple[guid];
            if (!multiple)
                _fileMap[guid].Clear();

            _fileMap[guid][id] = file;

            if (_addListeners.TryGetValue(guid, out Action<File>? listener))
            {
                listener.Invoke(file);
            }
        }

        public void Remove(Guid guid, int serial)
        {
            _fileMap[guid].Remove(serial, out File? file);
            if (file == null) return;

            _hashes[guid].Remove(file.Hash);

            if (_remListeners.TryGetValue(guid, out Action<File>? listener))
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