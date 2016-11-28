module Util

let inline isNull (x:^T when ^T : not struct) = obj.ReferenceEquals (x, null)