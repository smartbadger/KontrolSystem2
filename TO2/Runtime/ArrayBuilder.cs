﻿using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime;

public class ArrayBuilder<T>(long capacity) {
    private readonly List<T> elements = new((int)capacity);

    public long Length => elements.Count;

    public ArrayBuilder<T> Append(T element) {
        elements.Add(element);
        return this;
    }

    public T[] Result() {
        return [.. elements];
    }
}

public static class ArrayBuilder {
    public static ArrayBuilder<T> Create<T>(long capacity) {
        return new ArrayBuilder<T>(capacity);
    }
}

public static class ArrayBuilderOps {
    public static ArrayBuilder<T> AddTo<T>(ArrayBuilder<T> builder, T element) {
        return builder.Append(element);
    }
}
